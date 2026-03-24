import { redirect } from '@sveltejs/kit';

// This load function is automatically called by SvelteKit
export function load() {
    throw redirect(303, '/ingredients');
}