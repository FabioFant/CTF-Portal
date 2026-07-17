import { InjectionToken } from '@angular/core';

export interface CtfConfig {
  name: string;
  year: number;
}

const currentCtf : CtfConfig = {
    name: "Current CTF",
    year: 2026,
}

export const CTF_CONFIG = new InjectionToken<CtfConfig>('ctf-config', {
  providedIn: "root",
  factory: () => currentCtf
});