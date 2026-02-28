import adapter from '@sveltejs/adapter-auto'; 

export default {
  kit: {
    adapter: adapter(),
    paths: {
      base: process.env.NODE_ENV === 'production' ? '/MyResume' : ''
    }
  }
};
