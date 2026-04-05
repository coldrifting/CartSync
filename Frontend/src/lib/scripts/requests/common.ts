import {redirect} from "@sveltejs/kit";
import type {Cookies} from "@sveltejs/kit";
import ErrorResponse from "$lib/scripts/classes/ErrorResponse.js";

export const apiBaseUrl: string = "http://localhost:5164/api";
export const defaultUrl: string = '/recipes';

export function getToken(cookies: Cookies): string {
    return cookies.get('token') ?? "";
}

export async function checkForErrors(cookies: Cookies, response: Response): Promise<void> {
    if (response.status === 401) {
        console.error("Token expired. Redirecting to login page...");
        cookies.delete('token', cookieSettings);
        
        throw redirect(307, '/login');
    }
    if (!response.ok) {
        console.error(response.url);
        console.error(response.status);
        console.error(response.statusText);
        const errorObj: ErrorResponse = await response.json();
        const errorMsg: string = ErrorResponse.asString(errorObj);
        throw new Error(errorMsg);
    }
}

export async function getValue(formData: FormData, formElementName: string): Promise<string> {
    const elementValue: string | undefined = formData.get(formElementName) as string | undefined
    if (elementValue === undefined) {
        throw new Error(`Invalid Input for ${formElementName}: '${elementValue}'`);
    }
    
    return elementValue.trim();
}

export async function getValueOrUndefined(formData: FormData, formElementName: string): Promise<string | undefined> {
    return formData.get(formElementName) as string | undefined;
}

export async function getValueOrNull(formData: FormData, formElementName: string): Promise<string | null> {
    let val: string | null = formData.get(formElementName) as string | null;
    return val === "" ? null : val;
}

export async function getValueArray(formData: FormData, formElementName: string): Promise<string[]> {
    const elementValue: string[] | null = formData.getAll(formElementName) as string[] | null
    if (elementValue === null) {
        throw new Error(`Invalid Input: ${elementValue}`);
    }
    
    return elementValue;
}

export async function getValueNumber(formData: FormData, formElementName: string): Promise<number> {
    const elementValue: number = formData.get(formElementName) as number | null ?? -1
    if (elementValue === -1) {
        throw new Error(`Invalid Input: ${elementValue}`);
    }
    
    return elementValue;
}

export async function getValueBoolean(formData: FormData, formElementName: string): Promise<boolean> {
    const elementValue: string | null = formData.get(formElementName) as string | null
    return elementValue === "on";
}

export function isContentImage(content: string): boolean {
    return content.trim().split(' ').length === 1 && content.includes('/');
}

export const cookieSettings: any = {
    path: '/', // Makes the cookie available across the entire site
    httpOnly: true, // Prevents client-side JavaScript from reading the cookie
    sameSite: 'lax', // Mitigates CSRF attacks
    secure: false,
    maxAge: 60 * 60 * 4, // Cookie expiration (4 hours)
}