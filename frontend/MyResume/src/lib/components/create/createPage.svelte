<script>
  let currentStep = 1;
  let totalSteps = 5;
  
  function nextStep() {
    if (currentStep < totalSteps) {
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
  
  // Данные для навыков
  let selectedSkills = [];
  let customSkill = '';
  
  // Популярные навыки
  const popularSkills = [
    'JavaScript', 'Python', 'React', 'Node.js', 'TypeScript',
    'HTML/CSS', 'SQL', 'Git', 'UI/UX', 'Figma',
    'Project Management', 'Agile', 'Scrum', 'Marketing', 'SEO',
    'Copywriting', 'Data Analysis', 'DevOps', 'AWS', 'Docker'
  ];
  
  // Категории навыков
  const skillCategories = [
    {
      name: 'Frontend',
      skills: ['React', 'Vue', 'Angular', 'JavaScript', 'TypeScript', 'HTML/CSS', 'Svelte']
    },
    {
      name: 'Backend',
      skills: ['Node.js', 'Python', 'Java', 'C#', 'PHP', 'Go', 'Ruby']
    },
    {
      name: 'Дизайн',
      skills: ['Figma', 'UI/UX', 'Adobe XD', 'Photoshop', 'Illustrator', 'Sketch']
    },
    {
      name: 'Менеджмент',
      skills: ['Project Management', 'Agile', 'Scrum', 'Product Management', 'Team Leadership']
    }
  ];
  
  function toggleSkill(skill) {
    const index = selectedSkills.indexOf(skill);
    if (index === -1) {
      selectedSkills = [...selectedSkills, skill];
    } else {
      selectedSkills = selectedSkills.filter(s => s !== skill);
    }
  }
  
  function addCustomSkill() {
    if (customSkill.trim() && !selectedSkills.includes(customSkill.trim())) {
      selectedSkills = [...selectedSkills, customSkill.trim()];
      customSkill = '';
    }
  }
  
  function removeSkill(skill) {
    selectedSkills = selectedSkills.filter(s => s !== skill);
  }

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
  }

  // ========== ДАННЫЕ ДЛЯ ОПЫТА РАБОТЫ ==========
  let workExperience = [
    {
      id: Date.now(),
      company: '',
      position: '',
      startDate: '',
      endDate: '',
      current: false,
      description: ''
    }
  ];

  function addWorkExperience() {
    workExperience = [
      ...workExperience,
      {
        id: Date.now() + Math.random(),
        company: '',
        position: '',
        startDate: '',
        endDate: '',
        current: false,
        description: ''
      }
    ];
  }

  function removeWorkExperience(id) {
    if (workExperience.length > 1) {
      workExperience = workExperience.filter(item => item.id !== id);
    }
  }

  function toggleCurrentJob(item) {
    item.current = !item.current;
    if (item.current) {
      item.endDate = '';
    }
  }

  // ========== ДАННЫЕ ДЛЯ ОБРАЗОВАНИЯ ==========
  let education = [
    {
      id: Date.now() + 1,
      institution: '',
      degree: '',
      field: '',
      startDate: '',
      endDate: '',
      current: false
    }
  ];

  function addEducation() {
    education = [
      ...education,
      {
        id: Date.now() + Math.random() + 1,
        institution: '',
        degree: '',
        field: '',
        startDate: '',
        endDate: '',
        current: false
      }
    ];
  }

  function removeEducation(id) {
    if (education.length > 1) {
      education = education.filter(item => item.id !== id);
    }
  }

  function toggleCurrentEducation(item) {
    item.current = !item.current;
    if (item.current) {
      item.endDate = '';
    }
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

  // Годы для выбора (от 1980 до текущего)
  const currentYear = new Date().getFullYear();
  const years = Array.from({ length: currentYear - 1979 }, (_, i) => 1980 + i);
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
    </div>
  {/if}

  <!-- Шаг 2: Контакты -->
  {#if currentStep === 2}
    <div class="step-content">
      <h2 class="content-title">Начнем с основ</h2>
      <p class="content-subtitle">Работодатели должны знать, как с вами связаться.</p>
      
      <div class="form-grid">
        <div class="form-group">
          <label class="form-label">Имя</label>
          <input 
            type="text" 
            class="form-input" 
            bind:value={formData.firstName}
            placeholder="Иван"
          >
        </div>
        
        <div class="form-group">
          <label class="form-label">Фамилия</label>
          <input 
            type="text" 
            class="form-input" 
            bind:value={formData.lastName}
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
          <label class="form-label">Email</label>
          <input 
            type="email" 
            class="form-input" 
            bind:value={formData.email}
            placeholder="ivan@example.com"
          >
        </div>
        
        <div class="form-group">
          <label class="form-label">Телефон</label>
          <input 
            type="tel" 
            class="form-input" 
            bind:value={formData.phone}
            placeholder="+7 (999) 000-00-00"
          >
        </div>
        
        <div class="form-group full-width">
          <label class="form-label">О себе</label>
          <textarea 
            class="form-textarea" 
            bind:value={formData.about}
            placeholder="Расскажите о себе и своих целях..."
            rows="4"
          ></textarea>
        </div>
      </div>
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
                placeholder="Название компании"
              >
            </div>
            
            <div class="form-group full-width">
              <label class="form-label">Должность</label>
              <input 
                type="text" 
                class="form-input" 
                bind:value={exp.position}
                placeholder="Ваша должность"
              >
            </div>
            
            <div class="form-group">
              <label class="form-label">Начало работы</label>
              <div class="date-selects">
                <select class="form-select" bind:value={exp.startMonth}>
                  <option value="">Месяц</option>
                  {#each months as month, idx}
                    <option value={idx + 1}>{month}</option>
                  {/each}
                </select>
                <select class="form-select" bind:value={exp.startYear}>
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
                  <select class="form-select" bind:value={exp.endMonth}>
                    <option value="">Месяц</option>
                    {#each months as month, idx}
                      <option value={idx + 1}>{month}</option>
                    {/each}
                  </select>
                  <select class="form-select" bind:value={exp.endYear}>
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
                placeholder="Опишите ваши задачи, достижения, проекты..."
                rows="3"
              ></textarea>
            </div>
          </div>
        </div>
      {/each}
      
      <button class="add-block-btn" on:click={addWorkExperience}>
        + Добавить еще место работы
      </button>
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
                placeholder="Название университета, колледжа, школы"
              >
            </div>
            
            <div class="form-group">
              <label class="form-label">Степень</label>
              <select class="form-select" bind:value={edu.degree}>
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
                placeholder="Например: Программная инженерия"
              >
            </div>
            
            <div class="form-group">
              <label class="form-label">Начало обучения</label>
              <div class="date-selects">
                <select class="form-select" bind:value={edu.startMonth}>
                  <option value="">Месяц</option>
                  {#each months as month, idx}
                    <option value={idx + 1}>{month}</option>
                  {/each}
                </select>
                <select class="form-select" bind:value={edu.startYear}>
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
                  <select class="form-select" bind:value={edu.endMonth}>
                    <option value="">Месяц</option>
                    {#each months as month, idx}
                      <option value={idx + 1}>{month}</option>
                    {/each}
                  </select>
                  <select class="form-select" bind:value={edu.endYear}>
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
    </div>
  {/if}

  <!-- Шаг 5: Навыки -->
  {#if currentStep === 5}
    <div class="step-content">
      <h2 class="content-title">Ваши суперсилы</h2>
      <p class="content-subtitle">Выберите навыки из списка или добавьте свои</p>
      
      <!-- Выбранные навыки -->
      {#if selectedSkills.length > 0}
        <div class="selected-skills">
          <h3 class="section-subtitle">Выбранные навыки:</h3>
          <div class="skills-cloud">
            {#each selectedSkills as skill}
              <span class="skill-tag selected" on:click={() => removeSkill(skill)}>
                {skill} ✕
              </span>
            {/each}
          </div>
        </div>
      {/if}
      
      <!-- Популярные навыки (прямоугольнички) -->
      <div class="skills-section">
        <h3 class="section-subtitle">Популярные навыки</h3>
        <div class="skills-grid">
          {#each popularSkills as skill}
            <button 
              class="skill-rect {selectedSkills.includes(skill) ? 'selected' : ''}"
              on:click={() => toggleSkill(skill)}
            >
              {skill}
            </button>
          {/each}
        </div>
      </div>
      
      <!-- Категории навыков -->
      <div class="skills-section">
        <h3 class="section-subtitle">По категориям</h3>
        {#each skillCategories as category}
          <div class="skill-category">
            <h4 class="category-name">{category.name}</h4>
            <div class="skills-cloud">
              {#each category.skills as skill}
                <span 
                  class="skill-tag {selectedSkills.includes(skill) ? 'selected' : ''}"
                  on:click={() => toggleSkill(skill)}
                >
                  {skill}
                </span>
              {/each}
            </div>
          </div>
        {/each}
      </div>
      
      <!-- Добавление своего навыка -->
      <div class="custom-skill-section">
        <h3 class="section-subtitle">Добавить свой навык</h3>
        <div class="custom-skill-input">
          <input 
            type="text" 
            class="form-input" 
            bind:value={customSkill}
            placeholder="Введите название навыка..."
            on:keydown={(e) => e.key === 'Enter' && addCustomSkill()}
          >
          <button class="add-skill-btn" on:click={addCustomSkill}>
            Добавить
          </button>
        </div>
      </div>
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
      <button class="nav-button submit">
        СОЗДАТЬ РЕЗЮМЕ
      </button>
    {/if}
  </div>
</div>

<style>
  .resume-creator {
    max-width: 900px;
    margin: 0 auto;
    padding: 2rem;
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  }

  /* Шаги */
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

  /* Контент шага */
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
    font-size: 1.2rem;
    font-weight: 600;
    color: #1e293b;
    margin: 1.5rem 0 1rem 0;
  }

  /* Форма */
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

  /* Блоки опыта и образования */
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
    font-size: 1.1rem;
    font-weight: 600;
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

  /* Дата пикеры */
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

  /* Кнопки добавления */
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

  /* Стили для навыков */
  .selected-skills {
    background: #f0f9ff;
    padding: 1rem;
    border-radius: 12px;
    margin-bottom: 2rem;
  }

  .skills-section {
    margin-bottom: 2rem;
  }

  .skills-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(140px, 1fr));
    gap: 0.75rem;
    margin-top: 1rem;
  }

  .skill-rect {
    padding: 0.75rem 1rem;
    background: white;
    border: 2px solid #e2e8f0;
    border-radius: 8px;
    font-size: 0.95rem;
    font-weight: 500;
    color: #475569;
    cursor: pointer;
    transition: all 0.2s;
    text-align: center;
  }

  .skill-rect:hover {
    border-color: #2563eb;
    transform: translateY(-1px);
  }

  .skill-rect.selected {
    background: #2563eb;
    border-color: #2563eb;
    color: white;
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

  .skill-category {
    margin-bottom: 1.5rem;
  }

  .category-name {
    font-size: 1rem;
    font-weight: 600;
    color: #1e293b;
    margin: 0 0 0.5rem 0;
  }

  .custom-skill-section {
    margin-top: 2rem;
    padding: 1.5rem;
    background: #f8fafc;
    border-radius: 12px;
  }

  .custom-skill-input {
    display: flex;
    gap: 0.5rem;
  }

  .add-skill-btn {
    padding: 0 1.5rem;
    background: #2563eb;
    color: white;
    border: none;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    white-space: nowrap;
  }

  .add-skill-btn:hover {
    background: #1d4ed8;
  }

  /* Эксперты */
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
    font-size: 1.25rem;
    font-weight: bold;
    color: #1e293b;
    margin: 0;
  }

  .expert-desc {
    font-size: 0.95rem;
    color: #475569;
    margin: 0;
    line-height: 1.5;
  }

  /* Кнопки навигации */
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
    font-weight: 600;
    border: none;
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.2s;
  }

  .nav-button.prev {
    background: #f1f5f9;
    color: #475569;
  }

  .nav-button.prev:hover {
    background: #e2e8f0;
  }

  .nav-button.next {
    background: #2563eb;
    color: white;
  }

  .nav-button.next:hover {
    background: #1d4ed8;
    transform: translateX(2px);
  }

  .nav-button.submit {
    background: rgba(193, 18, 31, 0.63);
    color: white;
  }

  .nav-button.submit:hover {
    background: rgba(255, 0, 17, 0.63);
  }

  /* ===== АДАПТИВНОСТЬ ===== */

  @media (max-width: 1300px) {
    .resume-creator {
      max-width: 800px;
    }
  }

  @media (max-width: 1080px) {
    .content-title {
      font-size: 1.8rem;
    }
    
    .steps-container {
      gap: 0.5rem;
    }
    
    .step-title {
      font-size: 1rem;
    }
    
    .step-subtitle {
      font-size: 0.8rem;
    }
    
    .experts-grid {
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    }
  }

  @media (max-width: 900px) {
    .steps-container {
      flex-wrap: wrap;
      gap: 1rem;
    }
    
    .step-item {
      width: calc(50% - 0.5rem);
    }
    
    .form-grid {
      gap: 1rem;
    }
  }

  @media (max-width: 650px) {
    .resume-creator {
      padding: 1rem;
    }
    
    .steps-container {
      flex-direction: column;
      gap: 1rem;
    }
    
    .step-item {
      width: 100%;
    }
    
    .form-grid {
      grid-template-columns: 1fr;
    }
    
    .form-group.full-width {
      grid-column: span 1;
    }
    
    .content-title {
      font-size: 1.5rem;
    }
    
    .content-subtitle {
      font-size: 1rem;
    }
    
    .date-selects {
      grid-template-columns: 1fr;
    }
    
    .skills-grid {
      grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
    }
    
    .custom-skill-input {
      flex-direction: column;
    }
    
    .add-skill-btn {
      padding: 0.75rem;
    }
    
    .navigation-buttons {
      flex-direction: column;
    }
    
    .nav-button {
      width: 100%;
    }
  }

  @media (max-width: 475px) {
    .content-title {
      font-size: 1.25rem;
    }
    
    .content-subtitle {
      font-size: 0.9rem;
    }
    
    .step-number {
      width: 32px;
      height: 32px;
      font-size: 1rem;
    }
    
    .step-title {
      font-size: 0.9rem;
    }
    
    .step-subtitle {
      font-size: 0.7rem;
    }
    
    .form-input,
    .form-textarea,
    .form-select {
      padding: 0.6rem 0.8rem;
      font-size: 0.9rem;
    }
    
    .skill-rect {
      padding: 0.6rem;
      font-size: 0.85rem;
    }
    
    .skill-tag {
      padding: 0.4rem 0.8rem;
      font-size: 0.85rem;
    }
    
    .nav-button {
      font-size: 1rem;
      padding: 0.6rem 1.5rem;
    }
    
    .experience-block,
    .education-block {
      padding: 1rem;
    }
  }
</style>