"use client";

import AuthService, {
  RegisterRequest,
  LoginRequest,
} from "@/services/authService";
import { useAuthState } from "../../store/authStore";
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
        const response = await AuthService.register(user);
        return response?.data;
      },
      onError: (error: AxiosError) => {
        toast.error(error.message);
      },
      onSuccess: (data) => {
        const { message } = data;
        setIsopen && setIsopen(true);
        return {
          message,
        };
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
      const { status, data: responseData } = data;
      toast.success(status);
      setUser(responseData.user);
      setToken(responseData.jwtToken);
    },
  });

  const logout = ()=>{
    clearAuth();
    router.push("/login")
  };

  return {SignUpMutation, loginMutation, logout, user, token};
};


export default useAuth;