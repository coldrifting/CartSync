<script lang='ts'>
    import favicon from '$lib/assets/favicon.svg';
    import {Button, Col, Row, Container, Card, CardBody, CardFooter, CardTitle, FormGroup, Input} from '@sveltestrap/sveltestrap';
    import {enhance} from '$app/forms';
    let {form} = $props();
</script>

<svelte:head>
    <title>CartSync - Login</title>
    <link rel="icon" href={favicon}/>
</svelte:head>

<Container class="vh-100">
    <Row class="vh-100 justify-content-center align-items-center">
        <Col>
            <form class="d-flex justify-content-center align-items-center"
                  action="?/login"
                  use:enhance={() => {
                      
                return async ({ update }) => {
                    update({ reset: false });
                };
            }}
                  method="POST">
                <Card class="bg-modal login-container">
                    <CardBody>
                        <CardTitle class="mb-3">Login</CardTitle>
                        <FormGroup floating label="Username">
                            <Input name="username" required/>
                        </FormGroup>
                        <FormGroup floating label="Password">
                            <Input name="password" type="password" required/>
                        </FormGroup>

                    </CardBody>
                    <CardFooter>
                        <Row>
                            <Col sm="7">
                                {#if form?.error}
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