'use client';

import AuctionCard from "./AuctionCard";
import AppPagination from "../components/AppPagination";
import { useEffect, useState } from "react";
import { Auction, PagedResult } from "@/types";
import { getData } from "../actions/auctionActions";
import Filters from "./Filters";
import { useShallow } from "zustand/shallow";
import { useParamsStore } from "@/hooks/useParamsStore";
import qs from "query-string"
import EmptyFilter from "../components/EmptyFilter";

export default function Listings() {
    const [data, setData] = useState<PagedResult<Auction>>();
    const params = useParamsStore(useShallow(state => ({
        pageNumber: state.pageNumber,
        pageSize: state.pageSize,
        searchTerm: state.searchTerm,
        orderBy: state.orderBy,
        filterBy: state.filterBy
    })));

    const setParams = useParamsStore(state => state.setParams);
    const url = qs.stringifyUrl({ url: '', query: params }, { skipEmptyString: true });

    function setPageNUmber(pageNumber: number) {
        setParams({ pageNumber });
    }

    useEffect(() => {
        getData(url).then(data => {
            setData(data);
        })
    }, [url]);

    return (
        <>
            <Filters />
            {data && data.totalCount === 0 ? (
                <EmptyFilter showReset />
            ) : (
                <>
                    <div className="grid grid-cols-4 gap-6">
                        {data?.results && data.results.map(auction => (
                            <AuctionCard key={auction.id} auction={auction} />
                        ))}
                    </div>
                    <div className="flex justify-center mt-5">
                        <AppPagination
                            currentPage={params.pageNumber}
                            pageCount={data?.pageCount || 1}
                            pageChanged={setPageNUmber}
                        />
                    </div>
                </>
            )}

        </>
    )
}
