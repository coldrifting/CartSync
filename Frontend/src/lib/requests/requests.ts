import {redirect} from "@sveltejs/kit";
import type {Cookies} from "@sveltejs/kit";
import ErrorResponse from "$lib/types/ErrorResponse.js";

export const apiBaseUrl: string = "http://localhost:5164/api";

function getToken(cookies: Cookies): string {
    return cookies.get('token') ?? "";
}

export async function get(cookies: Cookies, url: string): Promise<Response> {
    const token = getToken(cookies);
    const response = await fetch(url, {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    });
    if (response.status === 401) {
        console.error("Token expired. Redirecting to login page...");
        throw redirect(307, '/login');
    }
    if (!response.ok) {
        const error: ErrorResponse = Object.assign(new ErrorResponse(), await response.json());
        const errorMsg = error.getErrorMsg();
        throw new Error(errorMsg);
    }
    return response;
}

export async function post(cookies: Cookies, url: string, body: any): Promise<Response> {
    const token = getToken(cookies);
    const response = await fetch(url, {
        method: 'POST',
        body: JSON.stringify(body),
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    });
    if (response.status === 401) {
        console.error("Token expired. Redirecting to login page...");
        throw redirect(307, '/login');
    }
    if (!response.ok) {
        const error: ErrorResponse = Object.assign(new ErrorResponse(), await response.json());
        const errorMsg = error.getErrorMsg();
        throw new Error(errorMsg);
    }
    return response;
}

export async function patch(cookies: Cookies, url: string, path: string, value: any): Promise<void> {
    const token = getToken(cookies);
    const patch =
        [
            {
                "op": "replace",
                "path": path,
                "value": value
            }
        ];

    const response = await fetch(url, {
        method: 'PATCH',
        body: JSON.stringify(patch),
        headers: {
            'Content-Type': 'application/json-patch+json',
            'Authorization': `Bearer ${token}`
        }
    });
    if (response.status === 401) {
        console.error("Token expired. Redirecting to login page...");
        throw redirect(307, '/login');
    }
    if (!response.ok) {
        const error: ErrorResponse = Object.assign(new ErrorResponse(), await response.json());
        const errorMsg = error.getErrorMsg();
        throw new Error(errorMsg);
    }
}

export async function del(cookies: Cookies, url: string): Promise<void> {
    const token = getToken(cookies);
    const response = await fetch(url, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    });
    if (response.status === 401) {
        console.error("Token expired. Redirecting to login page...");
        throw redirect(307, '/login');
    }
    if (!response.ok) {
        const error: ErrorResponse = Object.assign(new ErrorResponse(), await response.json());
        const errorMsg = error.getErrorMsg();
        throw new Error(errorMsg);
    }
}

export async function getValue(formData: FormData, formElementName: string): Promise<string> {
    const elementValue: string = formData.get(formElementName) as string | null ?? ""
    if (elementValue === '') {
        throw new Error(`Invalid Input: ${elementValue}`);
    }
    
    return elementValue.trim();
}