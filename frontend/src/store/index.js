// Global state management placeholder.
//
// Options:
//   - React Context + useReducer (built-in, no deps)
//   - Zustand       → npm install zustand
//   - Redux Toolkit → npm install @reduxjs/toolkit react-redux
//   - Jotai         → npm install jotai
//
// Example with React Context:
//
// import { createContext, useContext, useReducer } from 'react';
//
// const initialState = { user: null, theme: 'light' };
//
// function reducer(state, action) {
//   switch (action.type) {
//     case 'SET_USER': return { ...state, user: action.payload };
//     case 'SET_THEME': return { ...state, theme: action.payload };
//     default: return state;
//   }
// }
//
// const AppContext = createContext(null);
//
// export function AppProvider({ children }) {
//   const [state, dispatch] = useReducer(reducer, initialState);
//   return <AppContext.Provider value={{ state, dispatch }}>{children}</AppContext.Provider>;
// }
//
// export function useAppStore() {
//   return useContext(AppContext);
// }
