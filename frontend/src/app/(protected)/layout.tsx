import { redirect } from "next/navigation";
import { getCurrentUser } from "@/app/lib/api";

interface protectedLayerProps
{
  children : React.ReactNode;
}


export default async function ProtectedLayout({children} : protectedLayerProps)
{
  const user = await getCurrentUser();

  if(!user) redirect("/login");

  return (<>{children}</>);
}