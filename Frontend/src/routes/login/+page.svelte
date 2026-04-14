<script lang='ts'>
    import favicon from '$lib/assets/cartsync.svg';
    import {goto} from '$app/navigation';
    import {fail, redirect} from "@sveltejs/kit";
    import {browser} from "$app/environment";
    import FormInputText from "$lib/components/FormInputText.svelte";

    let username = $state('');
    let password = $state('');
    let isError = $state(false);

    async function handleSubmit(event: SubmitEvent) {
        event.preventDefault();

        const apiResponse = await fetch(`/api/user/login/cookie`, {
            method: 'POST',
            body: JSON.stringify({username, password}),
            headers: {'Content-Type': 'application/json'},
        });

        if (!apiResponse.ok) {
            isError = true;
            return fail(400, {
                username: username,
                error: 'Invalid credentials'
            });
        }

        if (browser) {
            await goto('/cart')
        }
        else {
            redirect(307, `/cart`);
        }
    }
      
    let usernameElement: HTMLInputElement | undefined = $state(undefined);
    let passwordElement: HTMLInputElement | undefined = $state(undefined);
    
    function handleEnter(e) {
        if (e.key === 'Enter' && (usernameElement?.value.trim() ?? '') !== '') {
            e.preventDefault(); // Stop form submission
            passwordElement?.focus();
        }
    }
    
</script>

<style>
    .login-window {
        height: 90dvh;
        max-height: 90dvh;
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
            <h2 class="p-3">Login</h2>
            <FormInputText id="username" 
                           label="Username" 
                           autocomplete="username" 
                           autocapitalize="none" 
                           spellcheck="false"
                           enterkeyhint="next"
                           onkeydown={handleEnter}
                           bind:value={username} 
                           bind:element={usernameElement}
                           required/>
            
            <FormInputText id="password" 
                           label="Password" 
                           type="password"
                           autocomplete="password"
                           enterkeyhint="go"
                           bind:value={password} 
                           bind:element={passwordElement}
                           required/>
            <div class="d-flex flex-column flex-sm-row align-items-center">
                {#if isError}
                    <h6 class="text-danger m-2">Invalid username or password</h6>
                {/if}
                <button class="btn btn-primary ms-auto m-2" type="submit">Login</button>
            </div>
        </form>
    </div>
</div>