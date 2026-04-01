import {redirect} from '@sveltejs/kit';
import {defaultUrl} from "$lib/scripts/requests/common.js";

// This load function is automatically called by SvelteKit
export function load() {
    throw redirect(303, defaultUrl);
}