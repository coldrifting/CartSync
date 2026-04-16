<script lang='ts'>
    import favicon from '$lib/assets/cartsync.svg';
    import {goto} from '$app/navigation';
    import {tick} from "svelte";
    import {fade} from 'svelte/transition';
    import {redirect} from "@sveltejs/kit";
    import {browser} from "$app/environment";
    import FormInputText from "$lib/components/FormInputText.svelte";
    import {Spinner} from "@sveltestrap/sveltestrap";

    let username = $state('');
    let password = $state('');
    let isError = $state(false);
    let isLoading = $state(false);

    async function handleSubmit(event: SubmitEvent) {
        event.preventDefault();

        isError = false;
        isLoading = true;
        try {
            const apiResponse = await fetch(`/api/user/login/cookie`, {
                method: 'POST',
                body: JSON.stringify({username, password}),
                headers: {'Content-Type': 'application/json'},
            });

            if (!apiResponse.ok) {
                isError = true;
                isLoading = false;
                tick().then(() => passwordElement?.focus());
                return;
            }

            if (browser) {
                await goto('/cart')
            } else {
                redirect(307, `/cart`);
            }
        }
        catch (error) {
            console.error(error);
        }
        isLoading = false;
    }
      
    let usernameElement: HTMLInputElement | undefined = $state(undefined);
    let passwordElement: HTMLInputElement | undefined = $state(undefined);
    
    function handleEnter(e: KeyboardEvent) {
        if (e.key === 'Enter' && (usernameElement?.value.trim() ?? '') !== '') {
            e.preventDefault(); // Stop form submission
            passwordElement?.focus();
        }
    }
</script>

<style>
    .login-window {
        height: 100dvh;
        max-height: 100dvh;
        background-color: rgba(0, 0, 0, 0.25);
    }
    
    .login-container {
        width: 100%;
        max-width: 576px;
        background-color: var(--theme-card-bg);
        
        button {
            width: 50%;
            @media(width < 576px) {
                width: 100%;
            }
        }
    }
    
    .login-info {
        height: 3rem;
    }
</style>

<svelte:head>
    <title>CartSync - Login</title>
    <link rel="icon" href={favicon}/>
    <meta name="theme-color" media="(prefers-color-scheme: dark)" content="#101828"/>
    <meta name="theme-color" media="(prefers-color-scheme: light)" content="#eeeeee"/>
</svelte:head>

<div class="d-flex justify-content-center align-items-center p-3 login-window">
    <div class="login-container p-3 rounded-3">
        <form onsubmit={handleSubmit}>
            <div class="d-flex flex-row align-items-center">
                <h2 class="p-3">Login</h2>
                {#if isLoading}
                    <div in:fade={{duration: 0, delay: 300}} class="ms-auto me-3">
                        <Spinner/>
                    </div>
                {/if}
            </div>
            <FormInputText id="username" 
                           label="Username" 
                           autocomplete="username" 
                           autocapitalize="none" 
                           spellcheck="false"
                           enterkeyhint="next"
                           onkeydown={handleEnter}
                           disabled={isLoading}
                           bind:value={username} 
                           bind:element={usernameElement}
                           required/>
            
            <FormInputText id="password" 
                           label="Password" 
                           type="password"
                           autocomplete="current-password"
                           enterkeyhint="go"
                           disabled={isLoading}
                           bind:value={password} 
                           bind:element={passwordElement}
                           required/>
            <div class="d-flex flex-column flex-sm-row align-items-center login-info">
                {#if isError}
                    <h6 in:fade={{duration: 0, delay: 100}} class="text-danger m-2">Invalid username or password</h6>
                {/if}
                <button class="btn btn-primary ms-auto m-2" disabled={isLoading} type="submit">Login</button>
            </div>
        </form>
    </div>
</div>