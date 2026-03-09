'use client';

import { useParamsStore } from "@/hooks/useParamsStore";
import { usePathname, useRouter } from "next/navigation";
import { AiOutlineCar } from "react-icons/ai";

export default function Logo() {
    const reset = useParamsStore(state => state.reset);
    const router = useRouter();
    const pathName= usePathname();

    function handleReset() {
        if (pathName !== '/') {
            router.push('/');
        }

        reset();
    }
    
    return (
        <div onClick={handleReset} className="cursor-pointer flex items-center gap-2 text-xl md:text-2xl lg:text-3xl font-semibold text-red-500 whitespace-nowrap">
            <AiOutlineCar className="w-6 h-6 md:w-8 md:h-8" />
            <div>Cars Auctions</div>
        </div>
    )
}
