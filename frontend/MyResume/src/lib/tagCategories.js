/**
 * Порядок запросов к GET /api/tags/category/{category}:
 * сначала IT (frontend, backend, database), затем остальные категории по алфавиту (en).
 * В UI попадают только категории, для которых API вернул непустой массив тегов.
 */
export const CATEGORY_FETCH_ORDER = [
  'frontend',
  'backend',
  'database',
  'culinary',
  'design',
  'education',
  'finance',
  'fitness',
  'healthcare',
  'hr',
  'law',
  'manual',
  'marketing',
  'photo',
  'sales'
];

/** Подписи секций в конструкторе и на странице поиска */
export const categoryDisplayNames = {
  frontend: 'Frontend',
  backend: 'Backend',
  database: 'Базы данных',
  marketing: 'Маркетинг',
  design: 'Дизайн',
  hr: 'HR и рекрутинг',
  education: 'Образование',
  sales: 'Продажи',
  manual: 'Ручной труд и производство',
  finance: 'Финансы',
  law: 'Юриспруденция',
  healthcare: 'Медицина и здоровье',
  culinary: 'Кулинария',
  fitness: 'Фитнес',
  photo: 'Фото и видео'
};

/**
 * @param {string} apiBaseUrl — например http://localhost:5052 (без завершающего слэша)
 * @returns {Promise<{ tagsByCategory: Record<string, any[]>, categoryOrder: string[], allTags: any[] }>}
 */
export async function loadTagsByCategories(apiBaseUrl) {
  const base = apiBaseUrl.replace(/\/$/, '');
  const tagsByCategory = {};
  const categoryOrder = [];
  const allTags = [];

  for (const category of CATEGORY_FETCH_ORDER) {
    try {
      const response = await fetch(
        `${base}/api/tags/category/${encodeURIComponent(category)}`
      );
      if (!response.ok) continue;

      const data = await response.json();
      if (!Array.isArray(data) || data.length === 0) continue;

      tagsByCategory[category] = data;
      categoryOrder.push(category);
      allTags.push(...data);
    } catch (e) {
      console.warn(`Не удалось загрузить теги категории «${category}»:`, e);
    }
  }

  return { tagsByCategory, categoryOrder, allTags };
}
