import { getDetailsViewData } from "@/app/actions/auctionActions";
import Heading from "@/app/components/Heading";
import CountdownTimer from "../../CountdownTimer";
import CarImage from "../../CarImage";
import DetailedSpecs from "./DetailedSpecs";
import EditButton from "./EditButton";
import { getCurrentUser } from "@/app/actions/authAction";
import DeleteButton from "./DeleteButton";
import BidList from "./BidList";

export default async function Details({ params }: { params: Promise<{ id: string }> }) {
    const { id } = await params;
    const data = await getDetailsViewData(id);
    const user = await getCurrentUser();

    return (
        <>
            <div className="flex flex-col md:flex-row justify-between gap-4 md:gap-0">
                <div className="flex items-center gap-3">
                    <Heading title={`${data.make} ${data.model}`} />
                    {user?.username === data.seller && (
                        <>
                            <EditButton id={id} />
                            <DeleteButton id={id} />
                        </>
                    )}
                </div>
                <div className="flex gap-3">
                    <h3 className="text-lg md:text-2xl font-semibold">Time remaining:</h3>
                    <CountdownTimer auctionEnd={data.auctionEnd} />
                </div>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 md:gap-6 mt-3">
                <div className="relative w-full bg-gray-200 aspect-16/10
                rounded-lg overflow-hidden">
                    <CarImage imageUrl={data.imageUrl} />
                </div>
               <BidList user={user} auction={data} />
            </div>

            <div className="mt-3 grid grid-cols-1 rounded-lg">
                <DetailedSpecs auction={data} />
            </div>
        </>
    )
}
