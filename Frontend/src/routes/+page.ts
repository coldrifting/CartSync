import {goto} from "$app/navigation";
import {browser} from "$app/environment";
import {redirect} from "@sveltejs/kit";

export async function load() {
    if (browser) {
        await goto('/cart');
    }
    else {
        redirect(307, '/cart');
    }
}