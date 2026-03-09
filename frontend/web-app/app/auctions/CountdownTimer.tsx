'use client';

import { useBidStore } from "@/hooks/useBidStore";
import dynamic from "next/dynamic";
import { usePathname } from "next/navigation";
import { zeroPad } from "react-countdown";

// Renderer callback with condition
const renderer = ({ days, hours, minutes, seconds, completed }:
    {days: number, hours: number, minutes: number, seconds: number, completed: boolean}
) => {

    return (
        <div className={`
            border-2 border-white text-white py-1 px-2 rounded-lg flex justify-center text-xs md:text-sm"
                ${completed ? "bg-red-600" : (days == 0 && hours < 10) 
                    ? 'bg-amber-600': 'bg-green-600'}
        `}>
            {completed ? (
                <span>Auction Finished</span>
            ) : (
                <span suppressHydrationWarning>{days}:{zeroPad(hours)}:{zeroPad(minutes)}:{zeroPad(seconds)}</span>
            )}
        </div>
    );
};

type Props = {
    auctionEnd: string
}

const Countdown = dynamic(() => import("react-countdown"), { ssr: false });

export default function CountdownTimer({ auctionEnd }: Props) {
    const setOpen = useBidStore(state => state.setOpen);
    const pathname = usePathname();

    function auctionFinished() {
        if (pathname.startsWith('/auctions/details/')) {
            setOpen(false);
        }
    }

    return (
        <div className="text-sm text-gray-500">
            <Countdown date={auctionEnd} renderer={renderer} onComplete={auctionFinished} />
        </div>
    )
};
