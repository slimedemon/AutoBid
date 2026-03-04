'use client';

import { placeBidForAuction } from "@/app/actions/auctionActions";
import { useBidStore } from "@/hooks/useBidStore";
import { numberWithCommas } from "@/lib/numberWithComma";
import { on } from "events";
import { FieldValues, useForm } from "react-hook-form";
import toast from "react-hot-toast";

type Props = {
    auctionId: string;
    highBid: number;
}

export default function BidForm({ auctionId, highBid }: Props) {
    const { register, handleSubmit, reset } = useForm();
    const addBid = useBidStore(state => state.addBid);

    function onSubmit(data: FieldValues) {
        if (data.amount <= highBid) {
            reset();
            return toast.error(`Bid must be higher than current high bid (${numberWithCommas(highBid)})`);
        }

        placeBidForAuction(auctionId, +data.amount).then(bid => {
            if (bid.error){
                reset();
                throw bid.error;
            }

            addBid(bid);
            reset();
        }).catch((error: any) => {
            toast.error(error.message);
        });
    }

    return (
        <form onSubmit={handleSubmit(onSubmit)}
            className="flex items-center border-2 rounded-lg py-2">
            <input
                {...register("amount", { required: true })}
                type="number"
                className="input-custom"
                placeholder={`Enter bid amount (minimum ${numberWithCommas(highBid + 1)})`} />

        </form>
    )
}
