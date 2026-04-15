import type ErrorResponse from "$lib/models/ErrorResponse.ts";
import ErrorCustom from "$lib/models/ErrorCustom.js";
import {goto} from "$app/navigation";
import {redirect} from "@sveltejs/kit";
import {browser} from '$app/environment'
import {createMutation, type QueryClient} from "@tanstack/svelte-query";

type fetchFunction = (input: (RequestInfo | URL), init?: RequestInit) => Promise<Response>;

async function checkForErrors(response: Response): Promise<void> {
    if (response.status === 401) {
        if (browser) {
            await goto('/login');
        }
        else {
            throw redirect(303, "/login");
        }
    }
    else if (!response.ok) {
        const errorObj: ErrorResponse = await response.json();
        console.error(errorObj);
        throw new ErrorCustom(errorObj);
    }
}

export async function get<T>(url: string, fetchFunc: fetchFunction | undefined = undefined): Promise<T> {
    const requestInit: RequestInit = {
        method: "GET",
        headers: {
            'Content-Type': 'application/json'
        }
    }
    const response: Response = fetchFunc !== undefined 
        ? await fetchFunc(url, requestInit) 
        : await fetch(url, requestInit);
    await checkForErrors(response);
    return await response.json()
}

export async function post(url: string, body: any, fetchFunc: fetchFunction | undefined = undefined): Promise<string | null> {
    const requestInit: RequestInit = {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body)
    }
    const response: Response = fetchFunc !== undefined 
        ? await fetchFunc(url, requestInit) 
        : await fetch(url, requestInit);
    await checkForErrors(response);
    if (response.status === 201) {
        let location: string | null = response.headers.get("Location");
        if (location) {
            return location.split('/').at(-1) ?? null;
        }
    }
    return null;
}

export async function postAndGetId(url: string, body: any, fetchFunc: fetchFunction | undefined = undefined): Promise<string> {
    const requestInit: RequestInit = {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body)
    }
    const response: Response = fetchFunc !== undefined 
        ? await fetchFunc(url, requestInit) 
        : await fetch(url, requestInit);
    await checkForErrors(response);
    
    return (response.headers.get("Location") ?? "").split('/').at(-1) ?? "";
}

export async function put(url: string, body: any, fetchFunc: fetchFunction | undefined = undefined): Promise<void> {
    const requestInit: RequestInit = {
        method: "PUT",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body)
    }
    const response: Response = fetchFunc !== undefined 
        ? await fetchFunc(url, requestInit) 
        : await fetch(url, requestInit);
    await checkForErrors(response);
}

export async function patch(url: string, pathValuePairs: Record<string, any>, fetchFunc: fetchFunction | undefined = undefined): Promise<void> {
    const patch = Object.entries(pathValuePairs).map(([key, value]) => {
        return {
            "op": "replace",
            "path": key,
            "value": value
        }
    })
    
    const requestInit: RequestInit = {
        method: "PATCH",
        headers: {
            'Content-Type': 'application/json-patch+json',
        },
        body: JSON.stringify(patch)
    }
    const response: Response = fetchFunc !== undefined 
        ? await fetchFunc(url, requestInit) 
        : await fetch(url, requestInit);
    await checkForErrors(response);
}

export async function del(url: string, fetchFunc: fetchFunction | undefined = undefined): Promise<void> {
    const requestInit: RequestInit = {
        method: "DELETE",
        headers: {
            'Content-Type': 'application/json'
        }
    }
    const response: Response = fetchFunc !== undefined 
        ? await fetchFunc(url, requestInit) 
        : await fetch(url, requestInit);
    await checkForErrors(response);
}

export function mutate<T, TQuery>(client: QueryClient,
                                  key: any,
                                  promise: (value: T) => Promise<any>,
                                  updateFunction: ((query: TQuery, value: T) => TQuery) | undefined = undefined) {
    return createMutation(() => ({
        mutationFn: (value: T) => promise(value),
        onMutate: updateFunction ? async (value: T) => {
            await client.cancelQueries({queryKey: key})

            const previous = client.getQueryData<TQuery>(key);
            if (previous) {
                client.setQueryData(key, updateFunction(previous, value));
            }

            return previous;
        } : undefined,
        onError: async (_, __, context: any) => {
            if (context?.previous) {
                client.setQueryData<TQuery>(key, context.previous)
            }
        },
        onSettled: _ => client.invalidateQueries({queryKey: key}),
    }))
}