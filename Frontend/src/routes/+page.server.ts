import { redirect } from '@sveltejs/kit';
import {defaultUrl} from "$lib/requests/requests.js";

// This load function is automatically called by SvelteKit
export function load() {
    throw redirect(303, defaultUrl);
}