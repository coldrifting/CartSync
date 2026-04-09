import {fail, redirect} from '@sveltejs/kit';
import type {Actions, PageServerLoad} from './$types';
import {apiBaseUrl, cookieSettings, defaultUrl} from "$lib/requests/common.js";

export const load: PageServerLoad = async ({cookies}) => {
    const token: string = cookies.get('token') ?? "";
    if (token !== "") {
        throw redirect(303, defaultUrl);
    }
};

export const actions: Actions = {
    login: async ({request, cookies}) => {
        const formData = await request.formData();
        const username = formData.get('username');
        const password = formData.get('password');

        const apiResponse = await fetch(`${apiBaseUrl}/user/login`, {
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

        const {token} = await apiResponse.json();

        cookies.set('token', token, cookieSettings);

        throw redirect(303, defaultUrl);
    },
};

