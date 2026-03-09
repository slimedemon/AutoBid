import { useParamsStore } from "@/hooks/useParamsStore";
import { Button, ButtonGroup } from "flowbite-react";
import { AiOutlineClockCircle, AiOutlineSortAscending } from "react-icons/ai";
import { BsFillStopCircleFill, BsStopwatchFill } from "react-icons/bs";
import { GiFinishLine, GiFlame,  } from "react-icons/gi";

const pageSizeButtons = [4, 8, 12, 16];
const orderButtons = [
    { label: 'Alphabetical', icon: AiOutlineSortAscending, value: 'make' },
    { label: 'End date', icon: AiOutlineClockCircle, value: 'endingsoon' },
    { label: 'Recently added', icon: BsFillStopCircleFill, value: 'new' },
]
const filterButtons = [
    { label: 'Live auctions', icon: GiFlame, value: 'live' },
    { label: 'Ending < 6 hours', icon: GiFinishLine, value: 'endingsoon' },
    { label: 'Completed', icon: BsStopwatchFill, value: 'finished' },
]

export default function Filters() {
    const pageSize = useParamsStore(state => state.pageSize);
    const orderBy = useParamsStore(state => state.orderBy);
    const filterBy = useParamsStore(state => state.filterBy);
    const setParams = useParamsStore(state => state.setParams);

    return (
        <div className="flex flex-col lg:flex-row justify-between items-start lg:items-center mb-4 gap-4">
            <div className="w-full lg:w-auto">
                <span className="uppercase text-xs md:text-sm text-gray-500 mr-2 block sm:inline mb-2 sm:mb-0">Filter By:</span>
                    <ButtonGroup outline className="flex-wrap">
                        {filterButtons.map((button) => {
                        const Icon = button.icon;
                        return (
                            <Button
                                key={button.value}
                                onClick={() => setParams({ filterBy: button.value })}
                                color={`${filterBy === button.value ? 'red' : 'gray'}`}
                                className="focus:ring-0 text-xs md:text-sm">
                                <Icon className="mr-1 md:mr-3 h-3 w-3 md:h-4 md:w-4" />
                                <span className="sm:inline">{button.label}</span>
                            </Button>
                        )
                    })}
                </ButtonGroup>
            </div>
            <div className="w-full lg:w-auto">
                <span className="uppercase text-xs md:text-sm text-gray-500 mr-2 block sm:inline mb-2 sm:mb-0">Order By:</span>
                <ButtonGroup outline className="flex-wrap">
                    {orderButtons.map((button) => {
                        const Icon = button.icon;
                        return (
                            <Button
                                key={button.value}
                                onClick={() => setParams({ orderBy: button.value })}
                                color={`${orderBy === button.value ? 'red' : 'gray'}`}
                                className="focus:ring-0 text-xs md:text-sm">
                                <Icon className="mr-1 md:mr-3 h-3 w-3 md:h-4 md:w-4" />
                                <span className="sm:inline">{button.label}</span>
                            </Button>
                        )
                    })}
                </ButtonGroup>
            </div>

            <div className="w-full lg:w-auto">
                <span className="uppercase text-xs md:text-sm text-gray-500 mr-2 block sm:inline mb-2 sm:mb-0">Page Size:</span>
                <ButtonGroup outline>
                    {pageSizeButtons.map((value, index) => (
                        <Button
                            key={index}
                            onClick={() => setParams({ pageSize: value })}
                            color={`${pageSize === value ? 'red' : 'gray'}`}
                            className="focus:ring-0 text-xs md:text-sm"
                        >
                            {value}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>
        </div>
    )
}
