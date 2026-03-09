'use client';

import { useParamsStore } from "@/hooks/useParamsStore";
import { ChangeEvent, useEffect, useState } from "react";
import { FaSearch } from "react-icons/fa";

export default function Search() {
    const setParams = useParamsStore(state => state.setParams);
    const searchTerm = useParamsStore(state => state.searchTerm);
    const [value, setValue] = useState(searchTerm);

    useEffect(() => {
        setValue(searchTerm);
    }, [searchTerm])

    function handleChange(e: ChangeEvent<HTMLInputElement>) {
        setValue(e.target.value);
    }

    function handleSearch() {
        setParams({ searchTerm: value });
    }

    return (
        <div className="flex w-full md:w-[60%] lg:w-[50%] items-center border-2 border-gray-300 rounded-full py-2 shadown-sm">
            <input
                onKeyDown={(e) => {
                    if (e.key === 'Enter') {
                        handleSearch();
                    }
                }}
                onChange={handleChange}
                value={value}
                type="text"
                placeholder="Search for cars by make, model or color."
                className="input-custom text-xs md:text-sm"
            />
            <button onClick={handleSearch}>
                <FaSearch className="bg-red-400 text-white rounded-full p-2 cursor-pointer mx-2 w-8 h-8 md:w-9 md:h-9" />
            </button>
        </div>
    )
}
