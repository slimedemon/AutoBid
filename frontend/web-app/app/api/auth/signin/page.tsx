'use client';

import { use } from 'react';
import Unauthorized from "@/app/components/Unauthorized";

export default function SignIn({searchParams}: {searchParams: Promise<{callbackUrl?: string}>}) {
  const params = use(searchParams);
  return (
    <Unauthorized showLogin callbackUrl={params.callbackUrl}/>
  )
}
