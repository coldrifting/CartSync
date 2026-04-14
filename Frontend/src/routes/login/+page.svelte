<script lang='ts'>
    import favicon from '$lib/assets/favicon.svg';
    import {
        Button,
        Col,
        Row,
        Container,
        Card,
        CardBody,
        CardFooter,
        CardTitle,
        FormGroup,
        Input
    } from '@sveltestrap/sveltestrap';
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
</script>

<style>
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
</svelte:head>

<div class="vh-100 vw-100 d-flex justify-content-center align-items-center p-3">
    <div class="login-container p-3 rounded-3">
        <form onsubmit={handleSubmit}>
            <h2 class="p-3">Login</h2>
            <FormInputText id="username" label="Username" bind:value={username} required />
            <FormInputText id="password" label="Password" bind:value={password} required type="password" />
            <div class="d-flex flex-column flex-sm-row align-items-center">
                {#if isError}
                    <h6 class="text-danger m-2">Invalid username or password</h6>
                {/if}
                <button class="btn btn-primary ms-auto m-2" type="submit">Login</button>
            </div>
        </form>
    </div>
</div>