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
        <div className="flex justify-between items-center mb-4">
            <div>
                <span className="uppercase text-sm text-gray-500 mr-2">Filter By:</span>
                    <ButtonGroup outline>
                        {filterButtons.map((button) => {
                        const Icon = button.icon;
                        return (
                            <Button
                                key={button.value}
                                onClick={() => setParams({ filterBy: button.value })}
                                color={`${filterBy === button.value ? 'red' : 'gray'}`}
                                className="focus:ring-0">
                                <Icon className="mr-3 h-4 w-4" size={16} />
                                {button.label}
                            </Button>
                        )
                    })}
                </ButtonGroup>
            </div>
            <div>
                <span className="uppercase text-sm text-gray-500 mr-2">Order By:</span>
                <ButtonGroup outline>
                    {orderButtons.map((button) => {
                        const Icon = button.icon;
                        return (
                            <Button
                                key={button.value}
                                onClick={() => setParams({ orderBy: button.value })}
                                color={`${orderBy === button.value ? 'red' : 'gray'}`}
                                className="focus:ring-0">
                                <Icon className="mr-3 h-4 w-4" size={16} />
                                {button.label}
                            </Button>
                        )
                    })}
                </ButtonGroup>
            </div>

            <div>
                <span className="uppercase text-sm text-gray-500 mr-2">Page Size:</span>
                <ButtonGroup>
                    {pageSizeButtons.map((value, index) => (
                        <Button
                            key={index}
                            onClick={() => setParams({ pageSize: value })}
                            color={`${pageSize === value ? 'red' : 'gray'}`}
                            className="focus:ring-0"
                        >
                            {value}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>
        </div>
    )
}
