"use client";

import { apiFetch } from "@/lib/api";
import { useState } from "react";

interface PrescriptionItemForm {
  drugName: string;
  dosage: string;
  frequency: string;
  durationDays: string;
  scheduleFlag: string;
}

interface PrescriptionItemDto {
  id: string;
  drugName: string;
  dosage?: string | null;
  frequency?: string | null;
  durationDays?: number | null;
  scheduleFlag: string;
}

interface PrescriptionDto {
  id: string;
  prescriptionNumber: string;
  visitId: string;
  issuedAt: string;
  items: PrescriptionItemDto[];
}

const SCHEDULE_FLAGS = ["None", "H", "H1", "X", "Narcotic"];

function emptyItem(): PrescriptionItemForm {
  return {
    drugName: "",
    dosage: "",
    frequency: "",
    durationDays: "",
    scheduleFlag: "None",
  };
}

export default function PrescriptionsPage() {
  const [tenant, setTenant] = useState("");
  const [visitId, setVisitId] = useState("");
  const [items, setItems] = useState<PrescriptionItemForm[]>([emptyItem()]);
  const [prescription, setPrescription] = useState<PrescriptionDto | null>(
    null,
  );
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  function updateItem(
    index: number,
    field: keyof PrescriptionItemForm,
    value: string,
  ) {
    setItems((prev) =>
      prev.map((item, i) => (i === index ? { ...item, [field]: value } : item)),
    );
  }

  function addItem() {
    setItems((prev) => [...prev, emptyItem()]);
  }

  function removeItem(index: number) {
    setItems((prev) => prev.filter((_, i) => i !== index));
  }

  async function loadExisting() {
    setError("");
    setLoading(true);
    try {
      const data = await apiFetch<PrescriptionDto>(
        `/api/prescriptions/by-visit/${visitId}`,
        tenant,
      );
      setPrescription(data);
    } catch (e) {
      setPrescription(null);
      setError(
        e instanceof Error
          ? e.message
          : "No prescription found for this visit.",
      );
    } finally {
      setLoading(false);
    }
  }

  async function submitPrescription() {
    setError("");
    setLoading(true);
    try {
      const payload = {
        visitId,
        items: items
          .filter((i) => i.drugName.trim() !== "")
          .map((i) => ({
            drugName: i.drugName,
            dosage: i.dosage || null,
            frequency: i.frequency || null,
            durationDays: i.durationDays ? Number(i.durationDays) : null,
            scheduleFlag: i.scheduleFlag,
          })),
      };

      await apiFetch<{ id: string }>(`/api/prescriptions`, tenant, {
        method: "POST",
        body: JSON.stringify(payload),
      });

      await loadExisting();
      setItems([emptyItem()]);
    } catch (e) {
      setError(
        e instanceof Error ? e.message : "Failed to create prescription.",
      );
    } finally {
      setLoading(false);
    }
  }

  return (
    <main className="max-w-3xl mx-auto p-8 space-y-6">
      <h1 className="text-2xl font-semibold">Doctor Prescription</h1>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
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
          <label className="block text-sm font-medium mb-1">OPD Visit Id</label>
          <input
            className="border rounded px-3 py-2 w-full"
            value={visitId}
            onChange={(e) => setVisitId(e.target.value)}
            placeholder="Visit GUID"
          />
        </div>
      </div>

      <button
        onClick={loadExisting}
        disabled={!tenant || !visitId || loading}
        className="bg-slate-800 text-white px-4 py-2 rounded disabled:opacity-50"
      >
        Load Existing Prescription
      </button>

      {error && <p className="text-red-600">{error}</p>}

      {prescription && (
        <section className="border rounded p-4 bg-emerald-50 space-y-2">
          <h2 className="font-semibold">{prescription.prescriptionNumber}</h2>
          <ul className="text-sm space-y-1">
            {prescription.items.map((item) => (
              <li key={item.id}>
                {item.drugName} — {item.dosage ?? "—"} · {item.frequency ?? "—"}{" "}
                · {item.durationDays ? `${item.durationDays} days` : "—"}
                {item.scheduleFlag !== "None" && (
                  <span className="ml-2 text-xs bg-red-100 text-red-700 px-1.5 py-0.5 rounded">
                    Schedule {item.scheduleFlag}
                  </span>
                )}
              </li>
            ))}
          </ul>
        </section>
      )}

      <section className="border rounded p-4 space-y-4">
        <h2 className="font-semibold">New Prescription</h2>

        {items.map((item, index) => (
          <div
            key={index}
            className="grid grid-cols-1 sm:grid-cols-6 gap-2 items-end border-b pb-3"
          >
            <div className="sm:col-span-2">
              <label className="block text-xs text-gray-500 mb-1">
                Drug Name
              </label>
              <input
                className="border rounded px-2 py-1.5 w-full text-sm"
                value={item.drugName}
                onChange={(e) => updateItem(index, "drugName", e.target.value)}
              />
            </div>
            <div>
              <label className="block text-xs text-gray-500 mb-1">Dosage</label>
              <input
                className="border rounded px-2 py-1.5 w-full text-sm"
                value={item.dosage}
                onChange={(e) => updateItem(index, "dosage", e.target.value)}
                placeholder="500mg"
              />
            </div>
            <div>
              <label className="block text-xs text-gray-500 mb-1">
                Frequency
              </label>
              <input
                className="border rounded px-2 py-1.5 w-full text-sm"
                value={item.frequency}
                onChange={(e) => updateItem(index, "frequency", e.target.value)}
                placeholder="1-0-1"
              />
            </div>
            <div>
              <label className="block text-xs text-gray-500 mb-1">
                Duration (days)
              </label>
              <input
                type="number"
                className="border rounded px-2 py-1.5 w-full text-sm"
                value={item.durationDays}
                onChange={(e) =>
                  updateItem(index, "durationDays", e.target.value)
                }
              />
            </div>
            <div className="flex gap-1 items-end">
              <div className="flex-1">
                <label className="block text-xs text-gray-500 mb-1">
                  Schedule
                </label>
                <select
                  className="border rounded px-2 py-1.5 w-full text-sm"
                  value={item.scheduleFlag}
                  onChange={(e) =>
                    updateItem(index, "scheduleFlag", e.target.value)
                  }
                >
                  {SCHEDULE_FLAGS.map((flag) => (
                    <option key={flag} value={flag}>
                      {flag}
                    </option>
                  ))}
                </select>
              </div>
              {items.length > 1 && (
                <button
                  onClick={() => removeItem(index)}
                  className="text-red-600 text-xs px-2 py-1.5"
                >
                  Remove
                </button>
              )}
            </div>
          </div>
        ))}

        <button onClick={addItem} className="text-sm text-slate-700 underline">
          + Add drug
        </button>

        <div>
          <button
            onClick={submitPrescription}
            disabled={!tenant || !visitId || loading}
            className="bg-emerald-700 text-white px-4 py-2 rounded disabled:opacity-50"
          >
            Save Prescription
          </button>
        </div>
      </section>
    </main>
  );
}
