<script>
  import { goto } from "$app/navigation";
  import { base } from '$app/paths';
  import { onMount } from 'svelte';
  import { auth } from '$lib/authStore';
  import { get } from 'svelte/store';
  import {
    loadTagsByCategories,
    categoryDisplayNames
  } from '$lib/tagCategories.js';
  import { beginRemoteLoad, endRemoteLoad } from '$lib/stores/remoteLoading.js';

  let currentStep = 1;
  let totalSteps = 5;
  let isSubmitting = false;
  let submitError = '';

  let aiModalOpen = false;
  let aiQuestion = '';
  let aiAnswer = '';
  let aiLoading = false;
  let aiError = '';

  // Валидационные ошибки для каждого шага
  let stepErrors = {
    step1: '',
    step2: '',
    step3: '',
    step4: '',
    step5: ''
  };

  // API базовый URL
  const API_BASE_URL = 'http://localhost:5052';

  // Динамические теги из БД (только категории с непустым ответом API)
  let tagsByCategory = {};
  /** Порядок секций: IT, затем прочие по алфавиту — совпадает с порядком успешных запросов */
  let categoryOrder = [];
  let allTags = [];
  let selectedSkills = [];

  async function refreshTagsFromApi() {
    beginRemoteLoad();
    try {
      const { tagsByCategory: byCat, categoryOrder: order, allTags: tags } =
        await loadTagsByCategories(API_BASE_URL);
      tagsByCategory = { ...byCat };
      categoryOrder = [...order];
      allTags = [...tags];
    } catch (err) {
      console.error('Ошибка загрузки тегов:', err);
    } finally {
      endRemoteLoad();
    }
  }

  // Преобразование названий навыков в ID тегов (строку через запятую)
  function getTagIdsFromSkills(skills) {
    if (!skills || skills.length === 0) return '';

    const tagIds = skills
            .map(skill => {
              const tag = allTags.find(t => t.name === skill);
              return tag ? tag.id : null;
            })
            .filter(id => id !== null);

    return tagIds.join(',');
  }

  function toggleSkill(skill) {
    const index = selectedSkills.indexOf(skill);
    if (index === -1) {
      selectedSkills = [...selectedSkills, skill];
    } else {
      selectedSkills = selectedSkills.filter(s => s !== skill);
    }
    validateStep5();
  }

  // ========== ВАЛИДАЦИЯ ШАГА 1 ==========
  function validateStep1() {
    if (!selectedExpert) {
      stepErrors.step1 = 'Выберите стиль резюме';
      return false;
    }
    stepErrors.step1 = '';
    return true;
  }

  // ========== ВАЛИДАЦИЯ ШАГА 2 ==========
  function validateEmail(email) {
    const emailRegex = /^[^\s@]+@([^\s@]+\.)+[^\s@]+$/;
    return emailRegex.test(email);
  }

  function validatePhone(phone) {
    if (!phone) return true;
    const phoneRegex = /^[\+\d\s\-\(\)]{10,20}$/;
    return phoneRegex.test(phone);
  }

  function validateStep2() {
    const errors = [];

    if (!formData.firstName.trim()) {
      errors.push('Имя обязательно');
    } else if (formData.firstName.trim().length < 2) {
      errors.push('Имя должно содержать минимум 2 символа');
    }

    if (!formData.lastName.trim()) {
      errors.push('Фамилия обязательна');
    } else if (formData.lastName.trim().length < 2) {
      errors.push('Фамилия должна содержать минимум 2 символа');
    }

    if (!formData.email.trim()) {
      errors.push('Email обязателен');
    } else if (!validateEmail(formData.email.trim())) {
      errors.push('Введите корректный email');
    }

    if (formData.phone && !validatePhone(formData.phone)) {
      errors.push('Введите корректный номер телефона');
    }

    stepErrors.step2 = errors.join(', ');
    return errors.length === 0;
  }

  // ========== ВАЛИДАЦИЯ ШАГА 3 (Опыт работы) ==========
  const MIN_YEAR = 1900;
  const MAX_YEAR = new Date().getFullYear();

  function validateDate(year, month, allowFuture = false) {
    if (!year || !month) return true;
    const yearNum = parseInt(year);
    const monthNum = parseInt(month);

    if (yearNum < MIN_YEAR) return false;
    if (!allowFuture && yearNum > MAX_YEAR) return false;
    if (monthNum < 1 || monthNum > 12) return false;

    return true;
  }

  function isDateBefore(startYear, startMonth, endYear, endMonth) {
    if (!startYear || !startMonth || !endYear || !endMonth) return true;

    const start = new Date(parseInt(startYear), parseInt(startMonth) - 1);
    const end = new Date(parseInt(endYear), parseInt(endMonth) - 1);

    return start <= end;
  }

  function validateStep3() {
    const errors = [];

    for (let i = 0; i < workExperience.length; i++) {
      const exp = workExperience[i];
      const blockErrors = [];

      if (exp.company && exp.company.length > 100) {
        blockErrors.push('название компании слишком длинное');
      }

      if (exp.position && exp.position.length > 100) {
        blockErrors.push('должность слишком длинная');
      }

      if (exp.startYear || exp.startMonth) {
        if (!exp.startYear) blockErrors.push('укажите год начала');
        if (!exp.startMonth) blockErrors.push('укажите месяц начала');

        if (exp.startYear && !validateDate(exp.startYear, exp.startMonth, false)) {
          blockErrors.push(`год начала должен быть от ${MIN_YEAR} до ${MAX_YEAR}`);
        }
      }

      if (!exp.current && (exp.endYear || exp.endMonth)) {
        if (!exp.endYear) blockErrors.push('укажите год окончания');
        if (!exp.endMonth) blockErrors.push('укажите месяц окончания');

        if (exp.endYear && !validateDate(exp.endYear, exp.endMonth, false)) {
          blockErrors.push(`год окончания должен быть от ${MIN_YEAR} до ${MAX_YEAR}`);
        }
      }

      if (exp.startYear && exp.startMonth && exp.endYear && exp.endMonth && !exp.current) {
        if (!isDateBefore(exp.startYear, exp.startMonth, exp.endYear, exp.endMonth)) {
          blockErrors.push('дата окончания не может быть раньше даты начала');
        }
      }

      if (blockErrors.length > 0) {
        errors.push(`Место работы ${i + 1}: ${blockErrors.join(', ')}`);
      }
    }

    stepErrors.step3 = errors.join('; ');
    return errors.length === 0;
  }

  // ========== ВАЛИДАЦИЯ ШАГА 4 (Образование) ==========
  function validateStep4() {
    const errors = [];

    for (let i = 0; i < education.length; i++) {
      const edu = education[i];
      const blockErrors = [];

      if (edu.institution && edu.institution.length > 200) {
        blockErrors.push('название слишком длинное');
      }

      if (edu.field && edu.field.length > 100) {
        blockErrors.push('специальность слишком длинная');
      }

      if (edu.startYear || edu.startMonth) {
        if (!edu.startYear) blockErrors.push('укажите год начала');
        if (!edu.startMonth) blockErrors.push('укажите месяц начала');

        if (edu.startYear && !validateDate(edu.startYear, edu.startMonth, false)) {
          blockErrors.push(`год начала должен быть от ${MIN_YEAR} до ${MAX_YEAR}`);
        }
      }

      if (!edu.current && (edu.endYear || edu.endMonth)) {
        if (!edu.endYear) blockErrors.push('укажите год окончания');
        if (!edu.endMonth) blockErrors.push('укажите месяц окончания');

        if (edu.endYear && !validateDate(edu.endYear, edu.endMonth, false)) {
          blockErrors.push(`год окончания должен быть от ${MIN_YEAR} до ${MAX_YEAR}`);
        }
      }

      if (edu.startYear && edu.startMonth && edu.endYear && edu.endMonth && !edu.current) {
        if (!isDateBefore(edu.startYear, edu.startMonth, edu.endYear, edu.endMonth)) {
          blockErrors.push('дата окончания не может быть раньше даты начала');
        }
      }

      if (blockErrors.length > 0) {
        errors.push(`Образование ${i + 1}: ${blockErrors.join(', ')}`);
      }
    }

    stepErrors.step4 = errors.join('; ');
    return errors.length === 0;
  }

  // ========== ВАЛИДАЦИЯ ШАГА 5 (Навыки) ==========
  function validateStep5() {
    if (selectedSkills.length === 0) {
      stepErrors.step5 = 'Выберите хотя бы один навык';
      return false;
    }
    stepErrors.step5 = '';
    return true;
  }

  // Общая валидация перед отправкой
  function validateAllSteps() {
    const step1Valid = validateStep1();
    const step2Valid = validateStep2();
    const step3Valid = validateStep3();
    const step4Valid = validateStep4();
    const step5Valid = validateStep5();

    return step1Valid && step2Valid && step3Valid && step4Valid && step5Valid;
  }

  function nextStep() {
    let currentValid = false;

    switch (currentStep) {
      case 1:
        currentValid = validateStep1();
        break;
      case 2:
        currentValid = validateStep2();
        break;
      case 3:
        currentValid = validateStep3();
        break;
      case 4:
        currentValid = validateStep4();
        break;
      case 5:
        currentValid = validateStep5();
        break;
    }

    if (currentValid && currentStep < totalSteps) {
      currentStep++;
    }
  }

  function prevStep() {
    if (currentStep > 1) {
      currentStep--;
    }
  }

  // Данные формы
  let formData = {
    firstName: '',
    lastName: '',
    desiredPosition: '',
    email: '',
    phone: '',
    about: ''
  };

  // Данные для экспертов
  let selectedExpert = null;
  const experts = [
    {
      id: 'serious',
      name: 'Супер серьёзный',
      icon: '👔',
      description: 'Строгий подход, академический стиль, фокус на цифрах и результатах.'
    },
    {
      id: 'funny',
      name: 'Шутливый',
      icon: '🥸',
      description: 'Креативный подход, легкий слог, выделяется из толпы.'
    },
    {
      id: 'modern',
      name: 'Современный',
      icon: '🚀',
      description: 'Трендовый дизайн, фокус на софт-скиллах и адаптивности.'
    }
  ];

  function selectExpert(id) {
    selectedExpert = id;
    validateStep1();
  }

  // Данные для опыта работы
  let workExperience = [
    {
      id: crypto.randomUUID ? crypto.randomUUID() : Date.now(),
      company: '',
      position: '',
      startMonth: '',
      startYear: '',
      endMonth: '',
      endYear: '',
      current: false,
      description: ''
    }
  ];

  function addWorkExperience() {
    workExperience = [
      ...workExperience,
      {
        id: crypto.randomUUID ? crypto.randomUUID() : Date.now() + Math.random(),
        company: '',
        position: '',
        startMonth: '',
        startYear: '',
        endMonth: '',
        endYear: '',
        current: false,
        description: ''
      }
    ];
  }

  function removeWorkExperience(id) {
    if (workExperience.length > 1) {
      workExperience = workExperience.filter(item => item.id !== id);
      validateStep3();
    }
  }

  function toggleCurrentJob(item) {
    item.current = !item.current;
    if (item.current) {
      item.endMonth = '';
      item.endYear = '';
    }
    validateStep3();
  }

  // Данные для образования
  let education = [
    {
      id: crypto.randomUUID ? crypto.randomUUID() : Date.now() + 1,
      institution: '',
      degree: '',
      field: '',
      startMonth: '',
      startYear: '',
      endMonth: '',
      endYear: '',
      current: false
    }
  ];

  function addEducation() {
    education = [
      ...education,
      {
        id: crypto.randomUUID ? crypto.randomUUID() : Date.now() + Math.random() + 1,
        institution: '',
        degree: '',
        field: '',
        startMonth: '',
        startYear: '',
        endMonth: '',
        endYear: '',
        current: false
      }
    ];
  }

  function removeEducation(id) {
    if (education.length > 1) {
      education = education.filter(item => item.id !== id);
      validateStep4();
    }
  }

  function toggleCurrentEducation(item) {
    item.current = !item.current;
    if (item.current) {
      item.endMonth = '';
      item.endYear = '';
    }
    validateStep4();
  }

  // Степени образования
  const degreeOptions = [
    'Среднее общее',
    'Среднее профессиональное',
    'Бакалавр',
    'Магистр',
    'Кандидат наук',
    'Доктор наук',
    'MBA',
    'Курсы переподготовки'
  ];

  // Месяцы для выбора дат
  const months = [
    'Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
    'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
  ];

  // Годы для выбора (от 1900 до текущего)
  const years = Array.from({ length: MAX_YEAR - MIN_YEAR + 1 }, (_, i) => MIN_YEAR + i).reverse();

  // ========== ФОРМИРОВАНИЕ ДАННЫХ ДЛЯ ОТПРАВКИ ==========
  function prepareResumeData() {
    // Форматируем даты для опыта работы
    const formattedWorkExperience = workExperience.map(exp => ({
      company: exp.company,
      position: exp.position,
      startDate: exp.startYear && exp.startMonth ? `${exp.startYear}-${String(exp.startMonth).padStart(2, '0')}` : null,
      endDate: exp.current ? null : (exp.endYear && exp.endMonth ? `${exp.endYear}-${String(exp.endMonth).padStart(2, '0')}` : null),
      current: exp.current,
      description: exp.description
    })).filter(exp => exp.company || exp.position);

    // Форматируем даты для образования
    const formattedEducation = education.map(edu => ({
      institution: edu.institution,
      degree: edu.degree,
      field: edu.field,
      startDate: edu.startYear && edu.startMonth ? `${edu.startYear}-${String(edu.startMonth).padStart(2, '0')}` : null,
      endDate: edu.current ? null : (edu.endYear && edu.endMonth ? `${edu.endYear}-${String(edu.endMonth).padStart(2, '0')}` : null),
      current: edu.current
    })).filter(edu => edu.institution);

    // Преобразуем навыки в ID тегов
    const tagIds = getTagIdsFromSkills(selectedSkills);

    return {
      ownerEmail: get(auth).user?.email || formData.email.trim(),
      ownerRole: get(auth).user?.role || 'user',
      firstName: formData.firstName.trim(),
      lastName: formData.lastName.trim(),
      desiredPosition: formData.desiredPosition.trim(),
      email: formData.email.trim(),
      phone: formData.phone.trim(),
      about: formData.about.trim(),
      expertStyle: selectedExpert,
      workExperience: formattedWorkExperience,
      education: formattedEducation,
      skills: selectedSkills,
      tagIds: tagIds,
      createdAt: new Date().toISOString(),
      template: 'modern'
    };
  }

  async function sendAiQuestion() {
    const q = aiQuestion.trim();
    if (!q) return;
    aiLoading = true;
    aiError = '';
    aiAnswer = '';
    beginRemoteLoad();
    try {
      const user = get(auth).user;
      const response = await fetch(`${API_BASE_URL}/api/ai/resume-consult`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          ownerRole: user?.role || 'user',
          question: q,
          resume: prepareResumeData()
        })
      });
      let data = {};
      try {
        data = await response.json();
      } catch {
        data = {};
      }
      if (!response.ok) {
        aiError = data.error || 'Сервис временно недоступен';
        if (import.meta.env.DEV && data.detail) {
          aiError += `\n\n${data.detail}`;
        }
        return;
      }
      aiAnswer = data.answer || '';
    } catch (e) {
      console.error(e);
      aiError = 'Сервис временно недоступен';
    } finally {
      aiLoading = false;
      endRemoteLoad();
    }
  }

  // ========== ОТПРАВКА ДАННЫХ НА СЕРВЕР ==========
  async function submitResume() {
    if (!validateAllSteps()) {
      submitError = 'Пожалуйста, исправьте ошибки в форме';
      return;
    }

    isSubmitting = true;
    submitError = '';

    beginRemoteLoad();
    try {
      const resumeData = prepareResumeData();

      const response = await fetch(`${API_BASE_URL}/api/resumes/generate`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(resumeData)
      });

      if (!response.ok) {
        let errorMessage = 'Ошибка при создании резюме';
        try {
          const errorData = await response.json();
          errorMessage = errorData.error || errorData.title || errorMessage;
        } catch(e) {
          const text = await response.text();
          errorMessage = text || errorMessage;
        }
        throw new Error(errorMessage);
      }

      const result = await response.json();

      goto(`${base}/profile`);

    } catch (error) {
      console.error('Ошибка:', error);
      submitError = error.message || 'Не удалось создать резюме. Попробуйте позже.';
    } finally {
      isSubmitting = false;
      endRemoteLoad();
    }
  }

  onMount(async () => {
    await refreshTagsFromApi();
  });
</script>

<div class="resume-creator">
  <!-- Шаги -->
  <div class="steps-container">
    <div class="step-item" class:active={currentStep === 1}>
      <span class="step-number">1</span>
      <div class="step-text">
        <span class="step-title">Стилистика</span>
        <span class="step-subtitle">Эксперт</span>
      </div>
    </div>

    <div class="step-item" class:active={currentStep === 2}>
      <span class="step-number">2</span>
      <div class="step-text">
        <span class="step-title">Контакты</span>
        <span class="step-subtitle">Кто вы?</span>
      </div>
    </div>

    <div class="step-item" class:active={currentStep === 3}>
      <span class="step-number">3</span>
      <div class="step-text">
        <span class="step-title">Опыт</span>
        <span class="step-subtitle">История работы</span>
      </div>
    </div>

    <div class="step-item" class:active={currentStep === 4}>
      <span class="step-number">4</span>
      <div class="step-text">
        <span class="step-title">Образование</span>
        <span class="step-subtitle">Учеба</span>
      </div>
    </div>

    <div class="step-item" class:active={currentStep === 5}>
      <span class="step-number">5</span>
      <div class="step-text">
        <span class="step-title">Навыки</span>
        <span class="step-subtitle">Суперсилы</span>
      </div>
    </div>
  </div>

  <!-- Шаг 1: Выбор эксперта -->
  {#if currentStep === 1}
    <div class="step-content">
      <h2 class="content-title">Выберите стиль резюме</h2>
      <p class="content-subtitle">Наш виртуальный эксперт поможет вам оформить резюме в выбранном ключе.</p>

      <div class="experts-grid">
        {#each experts as expert}
          <button
                  type="button"
                  class="expert-card {selectedExpert === expert.id ? 'selected' : ''}"
                  on:click={() => selectExpert(expert.id)}
          >
            <div class="expert-icon">{expert.icon}</div>
            <h3 class="expert-name">{expert.name}</h3>
            <p class="expert-desc">{expert.description}</p>
          </button>
        {/each}
      </div>

      {#if stepErrors.step1}
        <div class="error-message step-error">{stepErrors.step1}</div>
      {/if}
    </div>
  {/if}

  <!-- Шаг 2: Контакты -->
  {#if currentStep === 2}
    <div class="step-content">
      <h2 class="content-title">Начнем с основ</h2>
      <p class="content-subtitle">Работодатели должны знать, как с вами связаться.</p>

      <div class="form-grid">
        <div class="form-group">
          <label class="form-label">Имя *</label>
          <input
                  type="text"
                  class="form-input"
                  bind:value={formData.firstName}
                  on:input={validateStep2}
                  placeholder="Иван"
          >
        </div>

        <div class="form-group">
          <label class="form-label">Фамилия *</label>
          <input
                  type="text"
                  class="form-input"
                  bind:value={formData.lastName}
                  on:input={validateStep2}
                  placeholder="Иванов"
          >
        </div>

        <div class="form-group full-width">
          <label class="form-label">Желаемая должность</label>
          <input
                  type="text"
                  class="form-input"
                  bind:value={formData.desiredPosition}
                  placeholder="Например Старший Продуктовый Дизайнер"
          >
        </div>

        <div class="form-group">
          <label class="form-label">Email *</label>
          <input
                  type="email"
                  class="form-input"
                  bind:value={formData.email}
                  on:input={validateStep2}
                  placeholder="ivan@example.com"
          >
        </div>

        <div class="form-group">
          <label class="form-label">Телефон</label>
          <input
                  type="tel"
                  class="form-input"
                  bind:value={formData.phone}
                  on:input={validateStep2}
                  placeholder="+7 (999) 000-00-00"
          >
        </div>

        <div class="form-group full-width">
          <label class="form-label">О себе</label>
          <textarea
                  class="form-textarea"
                  bind:value={formData.about}
                  rows="4"
          ></textarea>
        </div>
      </div>

      {#if stepErrors.step2}
        <div class="error-message step-error">{stepErrors.step2}</div>
      {/if}
    </div>
  {/if}

  <!-- Шаг 3: Опыт работы -->
  {#if currentStep === 3}
    <div class="step-content">
      <h2 class="content-title">Опыт работы</h2>
      <p class="content-subtitle">Добавьте места работы</p>

      {#each workExperience as exp, index}
        <div class="experience-block">
          <div class="block-header">
            <h3 class="block-title">Место работы {index + 1}</h3>
            {#if workExperience.length > 1}
              <button
                      class="remove-block"
                      on:click={() => removeWorkExperience(exp.id)}
                      title="Удалить"
              >✕</button>
            {/if}
          </div>

          <div class="form-grid">
            <div class="form-group full-width">
              <label class="form-label">Компания</label>
              <input
                      type="text"
                      class="form-input"
                      bind:value={exp.company}
                      on:input={validateStep3}
                      placeholder="Название компании"
              >
            </div>

            <div class="form-group full-width">
              <label class="form-label">Должность</label>
              <input
                      type="text"
                      class="form-input"
                      bind:value={exp.position}
                      on:input={validateStep3}
                      placeholder="Ваша должность"
              >
            </div>

            <div class="form-group">
              <label class="form-label">Начало работы</label>
              <div class="date-selects">
                <select class="form-select" bind:value={exp.startMonth} on:change={validateStep3}>
                  <option value="">Месяц</option>
                  {#each months as month, idx}
                    <option value={idx + 1}>{month}</option>
                  {/each}
                </select>
                <select class="form-select" bind:value={exp.startYear} on:change={validateStep3}>
                  <option value="">Год</option>
                  {#each years as year}
                    <option value={year}>{year}</option>
                  {/each}
                </select>
              </div>
            </div>

            <div class="form-group">
              <label class="form-label">Окончание работы</label>
              <div class="date-selects">
                {#if !exp.current}
                  <select class="form-select" bind:value={exp.endMonth} on:change={validateStep3}>
                    <option value="">Месяц</option>
                    {#each months as month, idx}
                      <option value={idx + 1}>{month}</option>
                    {/each}
                  </select>
                  <select class="form-select" bind:value={exp.endYear} on:change={validateStep3}>
                    <option value="">Год</option>
                    {#each years as year}
                      <option value={year}>{year}</option>
                    {/each}
                  </select>
                {/if}
              </div>
              <label class="checkbox-label">
                <input
                        type="checkbox"
                        bind:checked={exp.current}
                        on:change={() => toggleCurrentJob(exp)}
                > По настоящее время
              </label>
            </div>

            <div class="form-group full-width">
              <label class="form-label">Описание обязанностей и достижений</label>
              <textarea
                      class="form-textarea"
                      bind:value={exp.description}
                      rows="3"
              ></textarea>
            </div>
          </div>
        </div>
      {/each}

      <button class="add-block-btn" on:click={addWorkExperience}>
        + Добавить еще место работы
      </button>

      {#if stepErrors.step3}
        <div class="error-message step-error">{stepErrors.step3}</div>
      {/if}
    </div>
  {/if}

  <!-- Шаг 4: Образование -->
  {#if currentStep === 4}
    <div class="step-content">
      <h2 class="content-title">Образование</h2>
      <p class="content-subtitle">Добавьте учебные заведения</p>

      {#each education as edu, index}
        <div class="education-block">
          <div class="block-header">
            <h3 class="block-title">Образование {index + 1}</h3>
            {#if education.length > 1}
              <button
                      class="remove-block"
                      on:click={() => removeEducation(edu.id)}
                      title="Удалить"
              >✕</button>
            {/if}
          </div>

          <div class="form-grid">
            <div class="form-group full-width">
              <label class="form-label">Учебное заведение</label>
              <input
                      type="text"
                      class="form-input"
                      bind:value={edu.institution}
                      on:input={validateStep4}
                      placeholder="Название университета, колледжа, школы"
              >
            </div>

            <div class="form-group">
              <label class="form-label">Степень</label>
              <select class="form-select" bind:value={edu.degree} on:change={validateStep4}>
                <option value="">Выберите степень</option>
                {#each degreeOptions as degree}
                  <option value={degree}>{degree}</option>
                {/each}
              </select>
            </div>

            <div class="form-group">
              <label class="form-label">Специальность</label>
              <input
                      type="text"
                      class="form-input"
                      bind:value={edu.field}
                      on:input={validateStep4}
                      placeholder="Например: Программная инженерия"
              >
            </div>

            <div class="form-group">
              <label class="form-label">Начало обучения</label>
              <div class="date-selects">
                <select class="form-select" bind:value={edu.startMonth} on:change={validateStep4}>
                  <option value="">Месяц</option>
                  {#each months as month, idx}
                    <option value={idx + 1}>{month}</option>
                  {/each}
                </select>
                <select class="form-select" bind:value={edu.startYear} on:change={validateStep4}>
                  <option value="">Год</option>
                  {#each years as year}
                    <option value={year}>{year}</option>
                  {/each}
                </select>
              </div>
            </div>

            <div class="form-group">
              <label class="form-label">Окончание обучения</label>
              <div class="date-selects">
                {#if !edu.current}
                  <select class="form-select" bind:value={edu.endMonth} on:change={validateStep4}>
                    <option value="">Месяц</option>
                    {#each months as month, idx}
                      <option value={idx + 1}>{month}</option>
                    {/each}
                  </select>
                  <select class="form-select" bind:value={edu.endYear} on:change={validateStep4}>
                    <option value="">Год</option>
                    {#each years as year}
                      <option value={year}>{year}</option>
                    {/each}
                  </select>
                {/if}
              </div>
              <label class="checkbox-label">
                <input
                        type="checkbox"
                        bind:checked={edu.current}
                        on:change={() => toggleCurrentEducation(edu)}
                > Обучаюсь до сих пор
              </label>
            </div>
          </div>
        </div>
      {/each}

      <button class="add-block-btn" on:click={addEducation}>
        + Добавить еще образование
      </button>

      {#if stepErrors.step4}
        <div class="error-message step-error">{stepErrors.step4}</div>
      {/if}
    </div>
  {/if}

  <!-- Шаг 5: Навыки -->
  {#if currentStep === 5}
    <div class="step-content">
      <h2 class="content-title">Ваши суперсилы</h2>
      <p class="content-subtitle">Выберите навыки из списка</p>

      <!-- Выбранные навыки -->
      {#if selectedSkills.length > 0}
        <div class="selected-skills">
          <div class="selected-skills-head">
            <span class="selected-skills-label">Выбрано</span>
            <span class="selected-skills-count">{selectedSkills.length}</span>
          </div>
          <p class="selected-skills-hint">Нажмите на тег, чтобы убрать навык</p>
          <div class="skills-cloud skills-cloud--selected">
            {#each selectedSkills as skill}
              <span class="skill-tag skill-tag--compact skill-tag--picked selected" on:click={() => toggleSkill(skill)}>
                <span class="skill-tag-text">{skill}</span>
                <span class="skill-tag-remove" aria-hidden="true">×</span>
              </span>
            {/each}
          </div>
        </div>
      {/if}

      <div class="skills-step-card">
        <p class="skills-hint">
          <span class="skills-hint-mark" aria-hidden="true"></span>
          <span class="skills-hint-text">Разворачивайте категории по одной — так проще ориентироваться в списке.</span>
        </p>

        <div class="skills-accordion">
        {#each categoryOrder as category, i}
          {#if tagsByCategory[category] && tagsByCategory[category].length > 0}
            <details class="skills-category-panel" open={i === 0}>
              <summary class="skills-category-summary">
                <span class="skills-category-title">{categoryDisplayNames[category] || category}</span>
                <span class="skills-category-meta">
                  <span class="skills-count">{tagsByCategory[category].length}</span>
                  <span class="skills-chevron" aria-hidden="true"><span class="skills-chevron-inner"></span></span>
                </span>
              </summary>
              <div class="skills-panel-body">
                <div class="skills-cloud skills-cloud--grid">
                  {#each tagsByCategory[category] as tag}
                    <span
                      class="skill-tag skill-tag--compact {selectedSkills.includes(tag.name) ? 'selected' : ''}"
                      on:click={() => toggleSkill(tag.name)}
                    >
                      {tag.name}
                    </span>
                  {/each}
                </div>
              </div>
            </details>
          {/if}
        {/each}
        </div>
      </div>

      {#if stepErrors.step5}
        <div class="error-message step-error">{stepErrors.step5}</div>
      {/if}
    </div>
  {/if}

  <!-- Сообщение об ошибке -->
  {#if submitError}
    <div class="error-message">
      {submitError}
    </div>
  {/if}

  <!-- Кнопки навигации -->
  <div class="navigation-buttons">
    {#if currentStep > 1}
      <button class="nav-button prev" on:click={prevStep}>
        ← НАЗАД
      </button>
    {/if}

    {#if currentStep < totalSteps}
      <button class="nav-button next" on:click={nextStep}>
        ДАЛЕЕ →
      </button>
    {:else}
      <button
              class="nav-button submit"
              on:click={submitResume}
              disabled={isSubmitting}
      >
        {#if isSubmitting}
          СОЗДАНИЕ...
        {:else}
          СОЗДАТЬ РЕЗЮМЕ
        {/if}
      </button>
    {/if}
  </div>
</div>

{#if $auth.user && ($auth.user.role === 'premium' || $auth.user.role === 'admin')}
  <button
    type="button"
    class="ai-fab"
    on:click={() => { aiModalOpen = true; aiError = ''; }}
    aria-haspopup="dialog"
  >
    🤖 Спросить ИИ
  </button>
{/if}

{#if aiModalOpen}
  <div
    class="ai-modal-backdrop"
    role="button"
    tabindex="-1"
    aria-label="Закрыть"
    on:click={() => (aiModalOpen = false)}
    on:keydown={(e) => e.key === 'Escape' && (aiModalOpen = false)}
  ></div>
  <div class="ai-modal" role="dialog" aria-modal="true" aria-labelledby="ai-modal-title">
    <div class="ai-modal-inner">
      <div class="ai-modal-head">
        <h2 id="ai-modal-title" class="ai-modal-title">ИИ-консультант по резюме</h2>
        <button type="button" class="ai-modal-close" on:click={() => (aiModalOpen = false)} aria-label="Закрыть">×</button>
      </div>
      <p class="ai-modal-lead">
        Задайте вопрос — в запрос уйдут уже заполненные поля черновика (имя, опыт, образование, навыки и т.д.).
      </p>
      <label class="ai-label" for="ai-q">Ваш вопрос</label>
      <textarea
        id="ai-q"
        class="ai-textarea"
        rows="3"
        placeholder="Например: как лучше описать опыт менеджера по продажам?"
        bind:value={aiQuestion}
        disabled={aiLoading}
      ></textarea>
      <div class="ai-modal-actions">
        <button type="button" class="ai-btn ai-btn-ghost" on:click={() => (aiModalOpen = false)} disabled={aiLoading}>Закрыть</button>
        <button type="button" class="ai-btn ai-btn-primary" on:click={sendAiQuestion} disabled={aiLoading || !aiQuestion.trim()}>
          {aiLoading ? 'Отправка…' : 'Отправить'}
        </button>
      </div>
      {#if aiError}
        <p class="ai-error" role="alert">{aiError}</p>
      {/if}
      {#if aiAnswer}
        <div class="ai-answer-wrap">
          <h3 class="ai-answer-title">Ответ</h3>
          <div class="ai-answer">{aiAnswer}</div>
        </div>
      {/if}
    </div>
  </div>
{/if}

<style>
  .resume-creator {
    max-width: 900px;
    width: 100%;
    margin: 0 auto;
    padding: clamp(0.85rem, 3vw, 2rem);
    box-sizing: border-box;
    overflow-x: hidden;
  }

  .steps-container {
    display: flex;
    justify-content: space-between;
    margin-bottom: 3rem;
    padding: 1rem 0;
    border-bottom: 2px solid #e2e8f0;
  }

  .step-item {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    opacity: 0.5;
    transition: all 0.3s;
  }

  .step-item.active {
    opacity: 1;
  }

  .step-number {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background: #e2e8f0;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.25rem;
    font-weight: bold;
    color: #64748b;
  }

  .step-item.active .step-number {
    background: #2563eb;
    color: white;
  }

  .step-text {
    display: flex;
    flex-direction: column;
  }

  .step-title {
    font-size: 1.1rem;
    font-weight: 600;
    color: #1e293b;
  }

  .step-subtitle {
    font-size: 0.9rem;
    color: #64748b;
  }

  .step-content {
    margin-bottom: 2rem;
  }

  .content-title {
    font-size: 2rem;
    font-weight: bold;
    margin: 0 0 0.5rem 0;
    color: #000;
  }

  .content-subtitle {
    font-size: 1.1rem;
    color: #475569;
    margin-bottom: 2rem;
  }

  .section-subtitle {
    font-family: var(--font-heading);
    font-size: 1.35rem;
    font-weight: 700;
    color: #1e293b;
    margin: 2rem 0 1.25rem 0;
    letter-spacing: -0.01em;
  }

  .form-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1.5rem;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
  }

  .form-group.full-width {
    grid-column: span 2;
  }

  .form-label {
    font-size: 1rem;
    font-weight: 600;
    color: #475569;
  }

  .form-input,
  .form-textarea,
  .form-select {
    padding: 0.75rem 1rem;
    font-size: 1rem;
    border: 2px solid #e2e8f0;
    border-radius: 8px;
    transition: all 0.2s;
    background: white;
    width: 100%;
    box-sizing: border-box;
  }

  .form-select {
    cursor: pointer;
    appearance: none;
    background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%23475569' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E");
    background-repeat: no-repeat;
    background-position: right 1rem center;
    background-size: 1rem;
  }

  .form-input:focus,
  .form-textarea:focus,
  .form-select:focus {
    outline: none;
    border-color: #2563eb;
    box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
  }

  .form-textarea {
    resize: vertical;
    min-height: 100px;
    font-family: inherit;
  }

  .experience-block,
  .education-block {
    background: #f8fafc;
    border-radius: 12px;
    padding: 1.5rem;
    margin-bottom: 1.5rem;
    border: 1px solid #e2e8f0;
  }

  .block-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1rem;
  }

  .block-title {
    font-family: var(--font-heading);
    font-size: 1.2rem;
    font-weight: 700;
    color: #1e293b;
    margin: 0;
  }

  .remove-block {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    border: none;
    background: #fee2e2;
    color: #ef4444;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1rem;
    transition: all 0.2s;
  }

  .remove-block:hover {
    background: #fecaca;
    transform: scale(1.1);
  }

  .date-selects {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 0.5rem;
  }

  .checkbox-label {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-size: 0.95rem;
    color: #475569;
    cursor: pointer;
    margin-top: 0.5rem;
  }

  .add-block-btn {
    background: none;
    border: 2px dashed #94a3b8;
    border-radius: 8px;
    padding: 1rem;
    width: 100%;
    font-size: 1rem;
    font-weight: 600;
    color: #475569;
    cursor: pointer;
    transition: all 0.2s;
  }

  .add-block-btn:hover {
    border-color: #2563eb;
    color: #2563eb;
    background: #f0f9ff;
  }

  .selected-skills {
    background: linear-gradient(145deg, rgba(255, 255, 255, 0.95) 0%, #eff6ff 55%, #e0f2fe 100%);
    padding: 1rem 1.1rem;
    border-radius: 16px;
    margin-bottom: 1.25rem;
    border: 1px solid rgba(102, 155, 188, 0.45);
    box-shadow:
      0 4px 20px rgba(29, 53, 87, 0.08),
      0 0 0 1px rgba(255, 255, 255, 0.6) inset;
  }

  .selected-skills-head {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 0.75rem;
    margin-bottom: 0.35rem;
  }

  .selected-skills-label {
    font-family: var(--font-heading);
    font-size: 0.82rem;
    font-weight: 700;
    letter-spacing: 0.06em;
    text-transform: uppercase;
    color: #457b9d;
  }

  .selected-skills-count {
    font-family: var(--font-heading);
    font-size: 1.1rem;
    font-weight: 800;
    color: #1d3557;
    min-width: 1.75rem;
    height: 1.75rem;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    background: linear-gradient(135deg, #fff 0%, #f0f9ff 100%);
    border-radius: 10px;
    border: 1px solid rgba(69, 123, 157, 0.35);
    box-shadow: 0 1px 4px rgba(29, 53, 87, 0.06);
  }

  .selected-skills-hint {
    margin: 0 0 0.65rem;
    font-size: 0.75rem;
    color: #64748b;
  }

  .skills-step-card {
    background: linear-gradient(180deg, #ffffff 0%, #f8fafc 100%);
    border-radius: 18px;
    padding: 1.1rem 1rem 1.2rem;
    border: 1px solid rgba(102, 155, 188, 0.35);
    box-shadow:
      0 8px 32px rgba(29, 53, 87, 0.07),
      0 1px 0 rgba(255, 255, 255, 0.9) inset;
  }

  .skills-hint {
    display: flex;
    align-items: flex-start;
    gap: 0.6rem;
    margin: 0 0 1rem;
    font-size: 0.8rem;
    color: #475569;
    line-height: 1.45;
  }

  .skills-hint-mark {
    flex-shrink: 0;
    width: 8px;
    height: 8px;
    margin-top: 0.32em;
    border-radius: 50%;
    background: linear-gradient(135deg, #457b9d, #669bbc);
    box-shadow: 0 0 0 4px rgba(102, 155, 188, 0.22);
  }

  .skills-hint-text {
    flex: 1;
    min-width: 0;
  }

  .skills-accordion {
    display: flex;
    flex-direction: column;
    gap: 0.55rem;
  }

  .skills-category-panel {
    border-radius: 14px;
    background: #fff;
    overflow: hidden;
    border: 1px solid #e8eef4;
    box-shadow: 0 2px 8px rgba(15, 23, 42, 0.04);
    transition: box-shadow 0.25s ease, border-color 0.25s ease;
  }

  .skills-category-panel[open] {
    border-color: rgba(69, 123, 157, 0.45);
    box-shadow:
      0 8px 28px rgba(29, 53, 87, 0.1),
      0 0 0 1px rgba(69, 123, 157, 0.12);
  }

  .skills-category-summary {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 0.75rem;
    padding: 0.65rem 1rem 0.65rem 0.85rem;
    cursor: pointer;
    list-style: none;
    font-family: var(--font-heading);
    font-weight: 700;
    font-size: 0.9rem;
    color: #1d3557;
    user-select: none;
    background: linear-gradient(90deg, #f8fafc 0%, #f1f5f9 100%);
    border-left: 3px solid transparent;
    transition: background 0.2s ease, border-color 0.2s ease;
  }

  .skills-category-summary::-webkit-details-marker {
    display: none;
  }

  .skills-category-summary:focus-visible {
    outline: 2px solid #457b9d;
    outline-offset: 2px;
    z-index: 1;
  }

  .skills-category-summary:hover {
    background: linear-gradient(90deg, #f0f9ff 0%, #f8fafc 100%);
  }

  .skills-category-panel[open] .skills-category-summary {
    background: linear-gradient(90deg, #eff6ff 0%, #f8fafc 100%);
    border-left-color: #457b9d;
  }

  .skills-category-title {
    flex: 1;
    min-width: 0;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    letter-spacing: -0.02em;
  }

  .skills-category-meta {
    display: flex;
    align-items: center;
    gap: 0.55rem;
    flex-shrink: 0;
  }

  .skills-count {
    font-size: 0.68rem;
    font-weight: 800;
    letter-spacing: 0.02em;
    color: #1d3557;
    background: linear-gradient(135deg, #e0f2fe 0%, #dbeafe 100%);
    padding: 0.2rem 0.5rem;
    border-radius: 999px;
    border: 1px solid rgba(69, 123, 157, 0.25);
  }

  .skills-chevron {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: 1.65rem;
    height: 1.65rem;
    border-radius: 50%;
    background: rgba(255, 255, 255, 0.85);
    border: 1px solid #e2e8f0;
    transition: background 0.2s ease, border-color 0.2s ease;
  }

  .skills-chevron-inner {
    display: block;
    width: 0.36rem;
    height: 0.36rem;
    border-right: 2px solid #457b9d;
    border-bottom: 2px solid #457b9d;
    transform: rotate(-45deg);
    margin-top: -0.18rem;
    transition: transform 0.25s ease, border-color 0.2s ease;
  }

  .skills-category-panel[open] .skills-chevron {
    background: linear-gradient(135deg, #457b9d, #1d3557);
    border-color: transparent;
  }

  .skills-category-panel[open] .skills-chevron-inner {
    border-color: #fff;
    transform: rotate(135deg);
    margin-top: 0.1rem;
  }

  .skills-panel-body {
    padding: 0.65rem 0.75rem 0.75rem;
    background: linear-gradient(180deg, #fafbfc 0%, #ffffff 40%);
    border-top: 1px solid rgba(226, 232, 240, 0.9);
    max-height: 12rem;
    overflow-y: auto;
    -webkit-overflow-scrolling: touch;
    scrollbar-width: thin;
    scrollbar-color: rgba(69, 123, 157, 0.45) #f1f5f9;
  }

  .skills-panel-body::-webkit-scrollbar {
    width: 6px;
  }

  .skills-panel-body::-webkit-scrollbar-thumb {
    background: linear-gradient(180deg, #94a3b8, #64748b);
    border-radius: 999px;
  }

  .skills-cloud {
    display: flex;
    flex-wrap: wrap;
    gap: 0.35rem;
    margin: 0;
  }

  .skills-cloud--selected {
    max-height: 6.75rem;
    overflow-y: auto;
    padding-right: 0.2rem;
    scrollbar-width: thin;
    scrollbar-color: rgba(69, 123, 157, 0.4) transparent;
  }

  .skills-cloud--grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(7.5rem, 1fr));
    gap: 0.4rem;
  }

  @media (min-width: 640px) {
    .skills-cloud--grid {
      grid-template-columns: repeat(auto-fill, minmax(8.25rem, 1fr));
    }
  }

  .skill-tag {
    padding: 0.5rem 1rem;
    background: #f1f5f9;
    border-radius: 20px;
    font-size: 0.9rem;
    font-weight: 500;
    color: #475569;
    cursor: pointer;
    transition: all 0.2s;
  }

  .skill-tag--compact {
    padding: 0.32rem 0.5rem;
    border-radius: 999px;
    font-size: 0.72rem;
    font-weight: 600;
    line-height: 1.3;
    text-align: center;
    word-break: break-word;
    hyphens: auto;
    border: 1px solid #e2e8f0;
    background: linear-gradient(180deg, #ffffff 0%, #f8fafc 100%);
    color: #334155;
    box-shadow: 0 1px 2px rgba(15, 23, 42, 0.04);
  }

  .skill-tag--picked {
    display: inline-flex;
    align-items: center;
    gap: 0.35rem;
    padding: 0.38rem 0.55rem 0.38rem 0.65rem;
    border-radius: 999px;
  }

  .skill-tag-remove {
    font-size: 0.85rem;
    font-weight: 700;
    opacity: 0.85;
    line-height: 1;
  }

  .skill-tag:hover {
    background: #e2e8f0;
  }

  .skill-tag--compact:hover {
    border-color: #94a3b8;
    box-shadow: 0 2px 8px rgba(29, 53, 87, 0.08);
    transform: translateY(-1px);
  }

  .skill-tag.selected {
    background: linear-gradient(135deg, #3b82f6 0%, #1d4ed8 100%);
    color: #fff;
    border-color: transparent;
    box-shadow: 0 2px 10px rgba(37, 99, 235, 0.35);
  }

  .skill-tag--compact.selected {
    border-color: rgba(255, 255, 255, 0.35);
  }

  .skill-tag--picked.selected {
    background: linear-gradient(135deg, #1d3557 0%, #457b9d 100%);
    box-shadow: 0 3px 14px rgba(29, 53, 87, 0.28);
  }

  .experts-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1.5rem;
  }

  .expert-card {
    background: white;
    border: 2px solid #e2e8f0;
    border-radius: 16px;
    padding: 2rem 1.5rem;
    cursor: pointer;
    transition: all 0.2s;
    text-align: center;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 1rem;
  }

  .expert-card:hover {
    border-color: #94a3b8;
    transform: translateY(-2px);
  }

  .expert-card.selected {
    border-color: #2563eb;
    background: #f0f9ff;
    box-shadow: 0 4px 12px rgba(37, 99, 235, 0.1);
  }

  .expert-icon {
    font-size: 3rem;
  }

  .expert-name {
    font-family: var(--font-heading);
    font-size: 1.35rem;
    font-weight: 800;
    color: #1e293b;
    margin: 0;
    letter-spacing: -0.02em;
  }

  .expert-desc {
    font-size: 0.95rem;
    color: #475569;
    margin: 0;
    line-height: 1.5;
  }

  .error-message {
    background: #fee2e2;
    border-left: 4px solid #ef4444;
    color: #b91c1c;
    padding: 1rem;
    margin: 1rem 0;
    border-radius: 8px;
  }

  .step-error {
    margin-top: 1rem;
  }

  .navigation-buttons {
    display: flex;
    justify-content: flex-end;
    gap: 1rem;
    margin-top: 2rem;
    padding-top: 2rem;
    border-top: 2px solid #e2e8f0;
  }

  .nav-button {
    padding: 0.75rem 2rem;
    font-size: 1.1rem;
    font-family: var(--font-heading);
    font-weight: 600;
    white-space: nowrap;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.2s;
  }

  .nav-button:disabled {
    opacity: 0.5;
    cursor: not-allowed;
  }

  .nav-button.prev {
    background: #f1f5f9;
    color: #475569;
  }

  .nav-button.prev:hover:not(:disabled) {
    background: #e2e8f0;
  }

  .nav-button.next {
    background: #2563eb;
    color: white;
  }

  .nav-button.next:hover:not(:disabled) {
    background: #1d4ed8;
    transform: translateX(2px);
  }

  .nav-button.submit {
    background: rgba(193, 18, 31, 0.63);
    color: white;
  }

  .nav-button.submit:hover:not(:disabled) {
    background: rgba(255, 0, 17, 0.63);
  }

  @media (max-width: 1300px) {
    .resume-creator { max-width: 800px; }
  }

  @media (max-width: 1080px) {
    .content-title { font-size: 1.8rem; }
    .steps-container { gap: 0.5rem; }
    .step-title { font-size: 1rem; }
    .step-subtitle { font-size: 0.8rem; }
    .experts-grid { grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); }
  }

  @media (max-width: 900px) {
    .steps-container { flex-wrap: wrap; gap: 1rem; }
    .step-item { width: calc(50% - 0.5rem); }
    .form-grid { gap: 1rem; }
  }

  @media (max-width: 650px) {
    .resume-creator { padding: 1rem; }
    .steps-container { flex-direction: column; gap: 1rem; }
    .step-item { width: 100%; }
    .form-grid { grid-template-columns: 1fr; }
    .form-group.full-width { grid-column: span 1; }
    .content-title { font-size: 1.5rem; }
    .content-subtitle { font-size: 1rem; }
    .date-selects { grid-template-columns: 1fr; }
    .navigation-buttons { flex-direction: column; }
    .nav-button { width: 100%; }
  }

  @media (max-width: 475px) {
    .content-title { font-size: 1.25rem; }
    .content-subtitle { font-size: 0.9rem; }
    .step-number { width: 32px; height: 32px; font-size: 1rem; }
    .step-title { font-size: 0.9rem; }
    .step-subtitle { font-size: 0.7rem; }
    .form-input, .form-textarea, .form-select { padding: 0.6rem 0.8rem; font-size: 0.9rem; }
    .skill-tag--compact {
      font-size: 0.66rem;
      padding: 0.22rem 0.35rem;
    }
    .nav-button { font-size: 1rem; padding: 0.6rem 1.5rem; }
    .experience-block, .education-block { padding: 1rem; }
  }

  .ai-fab {
    position: fixed;
    right: 1.25rem;
    bottom: 1.5rem;
    z-index: 2400;
    font-family: var(--font-heading);
    font-size: 0.95rem;
    font-weight: 700;
    padding: 0.75rem 1.1rem;
    border: none;
    border-radius: 999px;
    cursor: pointer;
    color: #fff;
    background: linear-gradient(135deg, #457b9d 0%, #1d3557 100%);
    box-shadow: 0 6px 24px rgba(29, 53, 87, 0.35);
    transition: transform 0.15s ease, box-shadow 0.2s ease;
  }

  .ai-fab:hover {
    transform: translateY(-2px);
    box-shadow: 0 10px 28px rgba(29, 53, 87, 0.4);
  }

  .ai-modal-backdrop {
    position: fixed;
    inset: 0;
    z-index: 2500;
    background: rgba(15, 23, 42, 0.45);
    backdrop-filter: blur(4px);
  }

  .ai-modal {
    position: fixed;
    inset: 0;
    z-index: 2501;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 1rem;
    pointer-events: none;
  }

  .ai-modal-inner {
    pointer-events: auto;
    width: 100%;
    max-width: 32rem;
    max-height: min(88vh, 36rem);
    overflow: auto;
    background: linear-gradient(180deg, #ffffff 0%, #f8fafc 100%);
    border-radius: 18px;
    border: 1px solid rgba(102, 155, 188, 0.35);
    box-shadow: 0 20px 60px rgba(29, 53, 87, 0.2);
    padding: 1.25rem 1.35rem 1.4rem;
  }

  .ai-modal-head {
    display: flex;
    align-items: flex-start;
    justify-content: space-between;
    gap: 0.75rem;
    margin-bottom: 0.5rem;
  }

  .ai-modal-title {
    margin: 0;
    font-family: var(--font-heading);
    font-size: 1.25rem;
    font-weight: 800;
    color: #1d3557;
    letter-spacing: -0.02em;
  }

  .ai-modal-close {
    flex-shrink: 0;
    width: 2rem;
    height: 2rem;
    border: none;
    border-radius: 10px;
    background: #f1f5f9;
    color: #475569;
    font-size: 1.35rem;
    line-height: 1;
    cursor: pointer;
    transition: background 0.15s ease;
  }

  .ai-modal-close:hover {
    background: #e2e8f0;
  }

  .ai-modal-lead {
    margin: 0 0 1rem;
    font-size: 0.85rem;
    color: #64748b;
    line-height: 1.45;
  }

  .ai-label {
    display: block;
    font-size: 0.78rem;
    font-weight: 700;
    text-transform: uppercase;
    letter-spacing: 0.06em;
    color: #457b9d;
    margin-bottom: 0.35rem;
  }

  .ai-textarea {
    width: 100%;
    box-sizing: border-box;
    border-radius: 12px;
    border: 1px solid #e2e8f0;
    padding: 0.65rem 0.75rem;
    font-family: inherit;
    font-size: 0.95rem;
    resize: vertical;
    min-height: 5rem;
    margin-bottom: 0.85rem;
  }

  .ai-textarea:focus {
    outline: 2px solid rgba(69, 123, 157, 0.45);
    outline-offset: 1px;
    border-color: #94a3b8;
  }

  .ai-modal-actions {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    justify-content: flex-end;
    margin-bottom: 0.75rem;
  }

  .ai-btn {
    font-family: var(--font-heading);
    font-weight: 700;
    font-size: 0.88rem;
    padding: 0.5rem 1rem;
    border-radius: 10px;
    cursor: pointer;
    border: none;
    transition: opacity 0.15s ease, transform 0.15s ease;
  }

  .ai-btn:disabled {
    opacity: 0.55;
    cursor: not-allowed;
  }

  .ai-btn-ghost {
    background: #f1f5f9;
    color: #475569;
  }

  .ai-btn-primary {
    background: linear-gradient(135deg, #457b9d, #1d3557);
    color: #fff;
  }

  .ai-error {
    margin: 0 0 0.75rem;
    font-size: 0.88rem;
    color: #b91c1c;
    background: #fef2f2;
    padding: 0.5rem 0.65rem;
    border-radius: 10px;
    border: 1px solid #fecaca;
  }

  .ai-answer-wrap {
    margin-top: 0.5rem;
    padding-top: 0.85rem;
    border-top: 1px solid #e2e8f0;
  }

  .ai-answer-title {
    margin: 0 0 0.5rem;
    font-size: 0.82rem;
    font-weight: 800;
    text-transform: uppercase;
    letter-spacing: 0.05em;
    color: #457b9d;
  }

  .ai-answer {
    font-size: 0.92rem;
    line-height: 1.55;
    color: #334155;
    white-space: pre-wrap;
  }
</style>