import { Button } from "flowbite-react";
import Heading from "./Heading";
import { useParamsStore } from "@/hooks/useParamsStore";

type Props = {
    title?: string;
    subtitle?: string;
    showReset?: boolean;
}

export default function EmptyFilter({
    title = 'No matches for this filter',
    subtitle = 'Try adjusting your search or filter criteria',
    showReset
}: Props) {
    const reset = useParamsStore((state) => state.reset)

    return (
        <div className="flex flex-col gap-2 items-center justify-center h-[40vh] shadow-lg">
            <Heading title={title} subtitle={subtitle} center />
            {showReset && (
                <Button outline onClick={reset}>
                    Reset filters
                </Button>
            )}
        </div>
    )
}
