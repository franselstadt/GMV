import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { ChakraProvider, extendTheme } from "@chakra-ui/react";
import App from "./App.tsx";
import { Brand } from "./globals/brand";
import { Messages } from "./globals/messages";
import "./index.css";

const theme = extendTheme({
  styles: {
    global: {
      body: {
        bg: Brand.PageBg,
        color: Brand.Text,
      },
    },
  },
  fonts: {
    heading: `'Segoe UI', 'Helvetica Neue', Arial, sans-serif`,
    body: `'Segoe UI', 'Helvetica Neue', Arial, sans-serif`,
  },
  colors: {
    gmv: {
      blue: Brand.Blue,
      dash: Brand.DashPink,
    },
  },
});

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <ChakraProvider theme={theme}>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Navigate to={`/route/${Messages.DefaultRouteCode}`} replace />} />
          <Route path="/route/:routeCode" element={<App />} />
          <Route path="*" element={<Navigate to={`/route/${Messages.DefaultRouteCode}`} replace />} />
        </Routes>
      </BrowserRouter>
    </ChakraProvider>
  </StrictMode>,
);
