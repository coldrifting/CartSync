import {redirect} from '@sveltejs/kit';
import {dev} from '$app/environment';

/*
export async function handle({event, resolve}) {
    if (dev && event.url.pathname === '/.well-known/appspecific/com.chrome.devtools.json') {
        return new Response(undefined, {status: 404});
    }

    // Check if user is still logged in
    const session = event.cookies.get('CartSyncCookie');

    if (!session) {
        if (event.url.pathname.startsWith('/login') || event.url.pathname.startsWith('/logout')) {
            return resolve(event);
        }

        // Redirect unauthenticated users trying to access protected routes
        throw redirect(303, '/login');
    }

    return resolve(event);
}

 */