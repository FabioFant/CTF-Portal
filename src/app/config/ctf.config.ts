import { InjectionToken } from '@angular/core';

export interface CtfConfig {
  name: string;
  year: number;
}

export const ctf : CtfConfig = {
    name: "Summer CTF",
    year: 2026,
}

export const CTF_CONFIG = new InjectionToken<CtfConfig>('ctf-config');