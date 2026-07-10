import Link from "next/link";

export default function Home() {
  return (
    <main className="max-w-xl mx-auto p-12 space-y-4">
      <h1 className="text-2xl font-semibold">Lokynex Health</h1>
      <ul className="space-y-2">
        <li>
          <Link href="/lab-billing" className="text-blue-600 underline">
            Lab Billing & Report
          </Link>
        </li>
        <li>
          <Link href="/prescriptions" className="text-blue-600 underline">
            Doctor Prescription
          </Link>
        </li>
      </ul>
    </main>
  );
}
