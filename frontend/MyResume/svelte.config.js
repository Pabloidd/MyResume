import adapter from '@sveltejs/adapter-vercel'; 

/** @type {import('@sveltejs/kit').Config} */
const config = {
  kit: {
    adapter: adapter(), // Убираем лишние опции
    paths: {
      base: process.env.NODE_ENV === 'production' ? '/MyResume' : ''
    }
  }
};

export default config;
