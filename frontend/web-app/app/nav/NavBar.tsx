'use client';

import { useSession } from "next-auth/react";
import LoginButton from "./LoginButton";
import Logo from "./Logo";
import Search from "./Search";
import UserActions from "./UserActions";

export default function NavBar() {
  const session = useSession();
  const user = session.data?.user;

  return (
    <header className="sticky top-0 z-50 flex flex-col md:flex-row justify-between bg-white p-3 md:p-5 items-center text-gray-800 shadow-md gap-3 md:gap-0">
      <Logo />
      <Search />
      {user ? (
        <UserActions user={user} />
      ) : (
        <LoginButton />
      )}
    </header>
  )
}
