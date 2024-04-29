import axiosConfig from "@/config/axios";
import { AxiosResponse } from "axios";

export interface RegisterRequest {
  firstname: string;
  lastname: string;
  email: string;
  password: string;
}

export interface RegisterResponse {
  status: string;
  messaage: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  status: string;
  data: ResponseData;
  token: string
  user: User
}

export interface ResponseData {
  jwtToken: string;
  user: User;
}

export interface User {
  id: string;
  firstname: string;
  lastname: string;
  email: string;
  bio: string;
  createdAt: string;
  updatedAt: string;
}

class AuthService {
  static register = async (
    requestBody: RegisterRequest
  ): Promise<AxiosResponse<RegisterResponse>> => {
    return await axiosConfig.post("auth/register", requestBody);
  };
  static login = async (
    requestBody: LoginRequest
  ): Promise<AxiosResponse<LoginResponse>> => {
    return await axiosConfig.post("auth/login", requestBody);
  };
}

export default AuthService;
