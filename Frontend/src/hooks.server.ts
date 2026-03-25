import { redirect } from '@sveltejs/kit';

export async function handle({ event, resolve }) {
  // Get the session token from cookies
  const session = event.cookies.get('token');
  
  if (!session) {
    if (event.url.pathname.startsWith('/login') || event.url.pathname.startsWith('/logout')) {
      return resolve(event);
    }
    
    // Redirect unauthenticated users trying to access protected routes
    throw redirect(303, '/login');
  }

  // If authenticated, you can fetch user data and put it in locals for access in load functions
  // event.locals.user = user;

  return resolve(event);
}