import adapter from '@sveltejs/adapter-static';

/** @type {import('@sveltejs/kit').Config} */
const config = {
	kit: {
		adapter: adapter({
			// default options are shown. On some platforms
			// these options are set automatically — see below
			pages: 'build/cartsync',
			assets: 'build/cartsync',
			fallback: 'index.html',
			precompress: false,
			strict: true
		})
	},
	compilerOptions: {
		experimental: {
			async: true
		}
	}
};

export default config;