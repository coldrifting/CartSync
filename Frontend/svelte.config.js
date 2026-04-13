import adapter from '@sveltejs/adapter-static';

/** @type {import('@sveltejs/kit').Config} */
const config = {
	kit: {
		adapter: adapter({
			// default options are shown. On some platforms
			// these options are set automatically — see below
			pages: '../API/CartSync/wwwroot',
			assets: '../API/CartSync/wwwroot',
			fallback: '200.html',
			precompress: false,
			strict: true
		})
	}
};

export default config;