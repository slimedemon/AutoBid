import Heading from "@/app/components/Heading";
import AuctionForm from "../AuctionForm";

export default function page() {
  return (
    <div className="mx-auto max-w-[75%] shadow-lg p-10 bg-white rounded-lg">
      <Heading title="sell your call!" subtitle="Please enter the details of your car" />
      <AuctionForm />
    </div>
  )
}
