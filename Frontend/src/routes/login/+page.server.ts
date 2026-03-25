import {fail, redirect} from '@sveltejs/kit';
import type {Actions, PageServerLoad} from './$types';

export const load: PageServerLoad = async ({cookies, url}) => {
    const token: string = cookies.get('token') ?? "";
    if (token != "") {
        throw redirect(303, '/ingredients');
    }
};

export const actions: Actions = {
    login: async ({request, cookies}) => {
        const formData = await request.formData();
        const username = formData.get('username');
        const password = formData.get('password');

        // **1. Authenticate with your backend API**
        // (Example fetch, replace with your actual API call)
        const apiResponse = await fetch('http://localhost:5164/api/user/login', {
            method: 'POST',
            body: JSON.stringify({username, password}),
            headers: {'Content-Type': 'application/json'},
        });

        if (!apiResponse.ok) {
            return fail(400, {
                username: username,
                error: 'Invalid credentials'
            });
        }

        const {token} = await apiResponse.json(); // Assume API returns { token: '...' }

        // **2. Set the JWT as an HttpOnly cookie**
        cookies.set('token', token, {
            path: '/', // Makes the cookie available across the entire site
            httpOnly: true, // Prevents client-side JavaScript from reading the cookie
            sameSite: 'lax', // Mitigates CSRF attacks
            secure: false,
            maxAge: 60 * 60 * 24 * 7, // Cookie expiration (e.g., 1 week)
        });

        // **3. Redirect the user**
        throw redirect(303, '/ingredients');
    },
};