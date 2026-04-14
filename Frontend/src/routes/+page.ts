import {redirect} from "@sveltejs/kit";
import {browser} from "$app/environment";
import {goto} from "$app/navigation";

export const ssr = false;

if (browser) {
    await goto('/cart');
} else {
    redirect(307, '/cart');
}