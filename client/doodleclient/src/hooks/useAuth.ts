"use client";

import AuthService, {
  RegisterRequest,
  LoginRequest,
} from "@/services/authService";
import { useAuthState } from "@/store/authStore";
import axiosResponseMessage from "@/lib/axiosResponseMessage";
import { useMutation, useQuery } from "@tanstack/react-query";
import { toast } from "sonner";
import { AxiosError } from "axios";
import { useRouter } from "next/navigation";

const useAuth = () => {
  const router = useRouter();
  const { setUser, setToken, user, token, clearAuth } = useAuthState();

  const SignUpMutation = (setIsopen?: (bool: boolean) => void) => {
    return useMutation({
      mutationFn: async (user: RegisterRequest) => {
        const reponse = await AuthService.register(user);
        return reponse?.data;
      },
      onError: (error: AxiosError) => {
        toast.error(error.message);
      },
      onSuccess: (data) => {
        const { status, messaage } = data;
        setIsopen && setIsopen(true);
        return { status, messaage };
      },
    });
  };

  const loginMutation = useMutation({
    mutationFn: async (user: LoginRequest) => {
      const response = await AuthService.login(user);
      return response?.data;
    },
    onError: (error: AxiosError) => {
      toast.error(error.message);
    },
    onSuccess: (data) => {
      const { user, token } = data;
      toast.success("login successful");
      setUser(user);
      setToken(token);
    },
  });

  const logOut = () => {
    clearAuth();
    router.push("/login");
  };

  return { SignUpMutation, loginMutation, user, token, logOut };
};

export default useAuth;
