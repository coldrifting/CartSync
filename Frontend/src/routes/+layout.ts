import { redirect } from '@sveltejs/kit';
import type { LayoutLoad } from './$types';
import Cookies from "js-cookie";

export const load: LayoutLoad = async ({url}) => {
    const timestampInSeconds: number = Math.trunc(Date.now() / 1000);
    const expireTime: string | undefined = Cookies.get("CartSyncCookieExpireTime");
    const isCookieExpired: boolean = expireTime === undefined || Number.parseInt(expireTime) <= timestampInSeconds;
    
    if (isCookieExpired && !url.pathname.startsWith("/login")) {
        throw redirect(307, `/login`);
    }
    
    if (!isCookieExpired && url.pathname.startsWith("/login")) {
        throw redirect(307, `/login`);
    }
    
    if (url.pathname === '/') {
        throw redirect(307, `/cart`);
    }
};

export const ssr = false;