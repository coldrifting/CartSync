import { redirect } from '@sveltejs/kit';
import type { Actions } from './$types';
import {cookieSettings} from "$lib/requests/requests.js";

export const actions: Actions = {
  default: async ({ cookies }) => {
    cookies.delete('token', cookieSettings);
    
    throw redirect(303, '/login');
  },
};