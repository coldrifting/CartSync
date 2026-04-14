import {goto} from "$app/navigation";
import {browser} from "$app/environment";
import {redirect} from "@sveltejs/kit";
import {post} from "$lib/functions/requests.js";

export async function load({fetch}) {
    await post('/api/user/logout/cookie', {}, fetch);
    
    if (browser) {
        await goto('/login');
    }
    else {
        redirect(307, '/login');
    }
}