import { Challenge } from '../models/challenge';

// Qualche termine a tema CTF per generare titoli realistici
const categories = ['Pwn', 'Web', 'Crypto', 'Forensics', 'Reverse', 'OSINT'];
const prefixes = ['Buffer Overflow', 'SQL Injection', 'RSA', 'Stegano', 'CrackMe', 'Trace'];
const suffixes = ['101', 'Advanced', 'Bypass', 'Madness', 'Exploit', 'Master'];

// Funzione generatrice
const generateMockChallenges = (count: number): Challenge[] => {
  const mockArray: Challenge[] = [];
  
  for (let i = 1; i <= count; i++) {
    // Sceglie elementi casuali per variare i dati
    const randomCat = categories[Math.floor(Math.random() * categories.length)];
    const randomPre = prefixes[Math.floor(Math.random() * prefixes.length)];
    const randomSuf = suffixes[Math.floor(Math.random() * suffixes.length)];
    
    // Genera una data casuale per l'anno corrente
    const randomMonth = Math.floor(Math.random() * 12);
    const randomDay = Math.floor(Math.random() * 28) + 1;

    mockArray.push({
      id: i,
      title: `${randomPre} ${randomSuf} #${i}`,
      category: randomCat,
      points: Math.floor(Math.random() * 5 + 1) * 100, // Multipli di 100 da 100 a 500
      solved: Math.random() > 0.8, // 20% di probabilità che sia già risolta di default
      dateAdded: new Date(2026, randomMonth, randomDay)
    });
  }
  
  return mockArray;
};

// Esportiamo la costante che contiene le 100 sfide generate
export const MOCK_CHALLENGES: Challenge[] = generateMockChallenges(100);