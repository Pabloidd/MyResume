<script>
  import { goto } from "$app/navigation";
  import { base } from '$app/paths';
  import { onMount } from 'svelte';
  import { auth } from '$lib/authStore';
  import { get } from 'svelte/store';

  let currentStep = 1;
  let totalSteps = 5;
  let isSubmitting = false;
  let submitError = '';

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

  // Динамические теги из БД
  let tagsByCategory = {};
  let allTags = [];
  let selectedSkills = [];

  // Категории для отображения
  const categoryDisplayNames = {
    'frontend': 'Frontend',
    'backend': 'Backend',
    'database': 'Базы данных'
  };

  const categoryOrder = ['frontend', 'backend', 'database'];

  // Загрузка тегов из БД для отображения
  async function loadTags() {
    try {
      const categories = ['frontend', 'backend', 'database'];

      for (const category of categories) {
        const response = await fetch(`${API_BASE_URL}/api/tags/category/${category}`);

        if (response.ok) {
          const data = await response.json();
          tagsByCategory[category] = data;
        }
      }
    } catch (err) {
      console.error('Ошибка загрузки тегов:', err);
    }
  }

  // Загрузка всех тегов для маппинга названий в ID
  async function loadAllTagsForMapping() {
    try {
      const categories = ['frontend', 'backend', 'database'];
      const all = [];

      for (const category of categories) {
        const response = await fetch(`${API_BASE_URL}/api/tags/category/${category}`);
        if (response.ok) {
          const data = await response.json();
          all.push(...data);
        }
      }

      allTags = all;
    } catch (err) {
      console.error('Ошибка загрузки тегов для маппинга:', err);
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

  // ========== ОТПРАВКА ДАННЫХ НА СЕРВЕР ==========
  async function submitResume() {
    if (!validateAllSteps()) {
      submitError = 'Пожалуйста, исправьте ошибки в форме';
      return;
    }

    isSubmitting = true;
    submitError = '';

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
    }
  }

  onMount(async () => {
    await loadTags();
    await loadAllTagsForMapping();
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
          <h3 class="section-subtitle">Выбранные навыки:</h3>
          <div class="skills-cloud">
            {#each selectedSkills as skill}
              <span class="skill-tag selected" on:click={() => toggleSkill(skill)}>
                {skill} ✕
              </span>
            {/each}
          </div>
        </div>
      {/if}

      <!-- Динамические теги по категориям из БД -->
      {#each categoryOrder as category}
        {#if tagsByCategory[category] && tagsByCategory[category].length > 0}
          <div class="skills-section">
            <h3 class="section-subtitle">{categoryDisplayNames[category] || category}</h3>
            <div class="skills-cloud">
              {#each tagsByCategory[category] as tag}
                <span
                        class="skill-tag {selectedSkills.includes(tag.name) ? 'selected' : ''}"
                        on:click={() => toggleSkill(tag.name)}
                >
                  {tag.name}
                </span>
              {/each}
            </div>
          </div>
        {/if}
      {/each}

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

<style>
  .resume-creator {
    max-width: 900px;
    margin: 0 auto;
    padding: 2rem;
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
    background: #f0f9ff;
    padding: 1rem;
    border-radius: 12px;
    margin-bottom: 2rem;
  }

  .skills-section {
    margin-bottom: 2rem;
  }

  .skills-cloud {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    margin: 1rem 0;
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

  .skill-tag:hover {
    background: #e2e8f0;
  }

  .skill-tag.selected {
    background: #2563eb;
    color: white;
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
    .skill-tag { padding: 0.4rem 0.8rem; font-size: 0.85rem; }
    .nav-button { font-size: 1rem; padding: 0.6rem 1.5rem; }
    .experience-block, .education-block { padding: 1rem; }
  }
</style>