import {sveltekit} from '@sveltejs/kit/vite';
import {defineConfig} from 'vite';

function sleep(delay: number) {
    const start = (new Date()).getTime();
    while ((new Date()).getTime() - start < delay) {
    }
}

export default defineConfig({
    plugins: [
        sveltekit()
    ],
    server: {
        proxy: {
            '/api': {
                target: 'http://localhost:4000',
                changeOrigin: true,
                    rewrite: (path) => {
                      sleep(1)
                      return path
                    },
            }
        }
    }
});
