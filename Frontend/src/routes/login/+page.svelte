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

<svelte:head>
    <title>CartSync - Login</title>
    <link rel="icon" href={favicon}/>
</svelte:head>

<Container class="vh-100">
    <Row class="vh-100 justify-content-center align-items-center">
        <Col>
            <form onsubmit={handleSubmit} class="d-flex justify-content-center align-items-center">
                <Card class="bg-modal login-container">
                    <CardBody>
                        <CardTitle class="mb-3">Login</CardTitle>
                        <FormGroup floating label="Username">
                            <Input name="username" bind:value={username} required/>
                        </FormGroup>
                        <FormGroup floating label="Password">
                            <Input name="password" type="password" bind:value={password} required/>
                        </FormGroup>

                    </CardBody>
                    <CardFooter>
                        <Row>
                            <Col sm="7">
                                {#if isError}
                                    <h6 class="text-danger m-6">Invalid username or password</h6>
                                {/if}
                            </Col>
                            <Col sm="5">
                                <Button color="primary" block type="submit">Login</Button>
                            </Col>
                        </Row>
                    </CardFooter>
                </Card>
            </form>
        </Col>
    </Row>
</Container>