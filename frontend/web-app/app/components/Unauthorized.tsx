import { Button } from "flowbite-react";
import Heading from "./Heading";
import { signIn } from "next-auth/react";

type Props = {
    showLogin?: boolean;
    callbackUrl?: string;
}

export default function Unauthorized({
    showLogin,
    callbackUrl
}: Props) {
    return (
        <div className="flex flex-col gap-2 items-center justify-center h-[40vh] shadow-lg">
            <Heading title="Unauthorized" subtitle="You must be logged in to access this page." center />
            {showLogin && (
                <Button outline onClick={() => signIn('id-server', {redirectTo: callbackUrl})}>
                    Login
                </Button>
            )}
        </div>
    )
}
