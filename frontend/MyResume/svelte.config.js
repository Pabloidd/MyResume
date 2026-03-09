import adapter from '@sveltejs/adapter-static';

const config = {
  kit: {
    adapter: adapter({
      pages: 'build',
      assets: 'build',
      fallback: 'index.html'
    }),
    paths: {
      base: process.env.NODE_ENV === 'production' ? '/MyResume' : ''
    }
  }
};

export default config;