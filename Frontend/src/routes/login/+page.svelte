<script>
	import { enhance } from '$app/forms';

	let { data, form } = $props();
</script>

<div class="flex h-screen">
    <div class="flex flex-col w-full">
        <div class="h-32 shrink"></div>
        <h1 class="text-4xl ml-8 font-semibold">Login</h1>
        <form class="text-lg p-10 pb-5 mb-10 mt-2 bg-gray-700 rounded-2xl" 
              method="POST" 
              action="?/login" 
              use:enhance={() => {
                return async ({ update }) => {
                  update({ reset: false });
                };}}>
            <div class="mb-3 flex flex-row h-9">
                <label class="w-30" for="username">Username</label>
                <input class="flex-1 p-3 rounded-lg bg-gray-900" 
                       name="username"
                       placeholder="Username"
                       required
                />
            </div>
            <div class="mb-3 flex flex-row h-9">
                <label class="w-30" for="password">Password</label>
                <input class="flex-1 p-3 rounded-lg bg-gray-900" 
                       name="password" 
                       type="password"
                       placeholder="Password"
                       required
                />
            </div>
            {#if form?.error}
                <div class="mb-3 text-right">
                    <span class="text-sm text-red-500">Invalid username or password</span>
                </div>
            {/if}
            <div class="mb-3 flex flex-row">
                <input type="submit" class="flex-1 h-10 rounded-lg bg-blue-500 hover:bg-blue-600 active:bg-blue-700" />
            </div>
        </form>
    </div>
</div>