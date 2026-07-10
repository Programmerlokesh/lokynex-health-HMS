"use client";

import { apiFetch } from "@/lib/api";
import { useState } from "react";

interface LabReportResult {
  parameterName: string;
  resultValue?: string | null;
  unit?: string | null;
  referenceRange?: string | null;
  isAbnormal: boolean;
  isCritical: boolean;
}

interface LabReportTest {
  testName: string;
  specimenType?: string | null;
  results: LabReportResult[];
}

interface LabReport {
  orderId: string;
  orderNumber: string;
  status: string;
  orderedAt: string;
  releasedAt?: string | null;
  patientName: string;
  patientUhid: string;
  patientAge?: number | null;
  patientGender: string;
  orderingDoctorName?: string | null;
  tests: LabReportTest[];
}

interface InvoiceItem {
  id: string;
  description: string;
  quantity: number;
  unitPrice: number;
  gstRatePct: number;
  lineTotal: number;
}

interface Invoice {
  id: string;
  invoiceNumber: string;
  invoiceDate: string;
  subtotal: number;
  cgstAmount: number;
  sgstAmount: number;
  totalAmount: number;
  paymentStatus: string;
  items: InvoiceItem[];
}

export default function LabBillingPage() {
  const [tenant, setTenant] = useState("");
  const [orderId, setOrderId] = useState("");
  const [gstRatePct, setGstRatePct] = useState(0);
  const [report, setReport] = useState<LabReport | null>(null);
  const [invoice, setInvoice] = useState<Invoice | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  async function loadReport() {
    setError("");
    setLoading(true);
    setInvoice(null);
    try {
      const data = await apiFetch<LabReport>(
        `/api/lab/orders/${orderId}/report`,
        tenant,
      );
      setReport(data);
    } catch (e) {
      setReport(null);
      setError(e instanceof Error ? e.message : "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  async function generateInvoice() {
    setError("");
    setLoading(true);
    try {
      const created = await apiFetch<{ id: string }>(
        `/api/lab/orders/${orderId}/invoice`,
        tenant,
        {
          method: "POST",
          body: JSON.stringify({ gstRatePct }),
        },
      );
      const inv = await apiFetch<Invoice>(
        `/api/billing/invoices/${created.id}`,
        tenant,
      );
      setInvoice(inv);
    } catch (e) {
      setError(e instanceof Error ? e.message : "Failed to generate invoice.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="max-w-3xl mx-auto p-8 space-y-6">
      <h1 className="text-2xl font-semibold">Lab Billing & Report</h1>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div>
          <label className="block text-sm font-medium mb-1">
            Tenant Subdomain
          </label>
          <input
            className="border rounded px-3 py-2 w-full"
            value={tenant}
            onChange={(e) => setTenant(e.target.value)}
            placeholder="e.g. citycare"
          />
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">Lab Order Id</label>
          <input
            className="border rounded px-3 py-2 w-full"
            value={orderId}
            onChange={(e) => setOrderId(e.target.value)}
            placeholder="Order GUID"
          />
        </div>
        <div>
          <label className="block text-sm font-medium mb-1">GST Rate %</label>
          <input
            type="number"
            className="border rounded px-3 py-2 w-full"
            value={gstRatePct}
            onChange={(e) => setGstRatePct(Number(e.target.value))}
          />
        </div>
      </div>

      <div className="flex gap-3">
        <button
          onClick={loadReport}
          disabled={!tenant || !orderId || loading}
          className="bg-slate-800 text-white px-4 py-2 rounded disabled:opacity-50"
        >
          Load Report
        </button>
        <button
          onClick={generateInvoice}
          disabled={!tenant || !orderId || loading}
          className="bg-emerald-700 text-white px-4 py-2 rounded disabled:opacity-50"
        >
          Generate Invoice
        </button>
      </div>

      {error && <p className="text-red-600">{error}</p>}

      {report && (
        <section className="border rounded p-4 space-y-3">
          <h2 className="font-semibold">
            {report.orderNumber} — {report.status}
          </h2>
          <p className="text-sm text-gray-600">
            {report.patientName} ({report.patientUhid}) · Age{" "}
            {report.patientAge ?? "—"} · {report.patientGender}
          </p>
          {report.orderingDoctorName && (
            <p className="text-sm text-gray-600">
              Ordered by Dr. {report.orderingDoctorName}
            </p>
          )}

          {report.tests.map((test, i) => (
            <div key={i} className="border-t pt-3">
              <h3 className="font-medium">
                {test.testName}{" "}
                {test.specimenType ? `(${test.specimenType})` : ""}
              </h3>
              <table className="w-full text-sm mt-2">
                <thead>
                  <tr className="text-left text-gray-500">
                    <th className="pr-4">Parameter</th>
                    <th className="pr-4">Result</th>
                    <th className="pr-4">Unit</th>
                    <th className="pr-4">Reference Range</th>
                    <th>Flag</th>
                  </tr>
                </thead>
                <tbody>
                  {test.results.map((r, j) => (
                    <tr
                      key={j}
                      className={
                        r.isCritical
                          ? "text-red-600"
                          : r.isAbnormal
                            ? "text-amber-600"
                            : ""
                      }
                    >
                      <td className="pr-4">{r.parameterName}</td>
                      <td className="pr-4">{r.resultValue ?? "—"}</td>
                      <td className="pr-4">{r.unit ?? "—"}</td>
                      <td className="pr-4">{r.referenceRange ?? "—"}</td>
                      <td>
                        {r.isCritical
                          ? "Critical"
                          : r.isAbnormal
                            ? "Abnormal"
                            : "Normal"}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ))}
        </section>
      )}

      {invoice && (
        <section className="border rounded p-4 space-y-2 bg-emerald-50">
          <h2 className="font-semibold">Invoice {invoice.invoiceNumber}</h2>
          <ul className="text-sm">
            {invoice.items.map((item) => (
              <li key={item.id} className="flex justify-between">
                <span>
                  {item.description} (Qty {item.quantity})
                </span>
                <span>₹{item.lineTotal.toFixed(2)}</span>
              </li>
            ))}
          </ul>
          <div className="border-t pt-2 text-sm space-y-1">
            <div className="flex justify-between">
              <span>Subtotal</span>
              <span>₹{invoice.subtotal.toFixed(2)}</span>
            </div>
            <div className="flex justify-between">
              <span>CGST</span>
              <span>₹{invoice.cgstAmount.toFixed(2)}</span>
            </div>
            <div className="flex justify-between">
              <span>SGST</span>
              <span>₹{invoice.sgstAmount.toFixed(2)}</span>
            </div>
            <div className="flex justify-between font-semibold">
              <span>Total</span>
              <span>₹{invoice.totalAmount.toFixed(2)}</span>
            </div>
          </div>
          <p className="text-sm text-gray-600">
            Payment status: {invoice.paymentStatus}
          </p>
        </section>
      )}
    </main>
  );
}
