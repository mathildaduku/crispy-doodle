"use client";

import Link from "next/link";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import * as z from "zod";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import Image from "next/image";
import useAuth from "@/hooks/useAuth";
import { Loader2 } from "lucide-react";
import { AuthDialog } from "./auth-alert";
import { useState } from "react";

const registerFormSchema = z
  .object({
    firstname: z.string().min(2, {
      message: "First name must contain at least 2 character(s)",
    }),
    lastname: z.string().min(2, {
      message: "Last Name must contain at least 2 character(s)",
    }),
    email: z.string().email({
      message: "Invalid email",
    }),
    password: z.string().min(8, {
      message: "Password must be at least 8 characters",
    }),
    confirmPassword: z
      .string()
      .min(1, { message: "Confirm Password is required" }),
  })
  .refine((data) => data.password === data.confirmPassword, {
    path: ["confirmPassword"],
    message: "Password don't match",
  });

type RegisterFormValues = z.infer<typeof registerFormSchema>;

const RegisterPage = () => {
  const { SignUpMutation } = useAuth();
  const [isOpen, setIsOpen] = useState(false);
  const [registrationResult, setRegistrationResult] = useState<{
    status?: string;
    message?: string;
  }>({});

  const signUpMutation = SignUpMutation((bool: boolean) => setIsOpen(bool));

  const form = useForm<z.infer<typeof registerFormSchema>>({
    resolver: zodResolver(registerFormSchema),
    defaultValues: {
      firstname: "",
      lastname: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
  });

  const isLoading = signUpMutation.isPending;

  const onSubmit = async (data: RegisterFormValues) => {
    const { confirmPassword, ...dataWithoutConfirmPassword } = data;
    const mutationResult = await signUpMutation.mutateAsync(
      dataWithoutConfirmPassword
    );

    // console.log(mutationResult?.status);
    setRegistrationResult(mutationResult);
    form.reset();
  };

  return (
    <div>
      <section className="flex bg-primary opacity-90 justify-between ">
        <div className="w-[40%] hidden md:flex items-center justify-center">
          <div className="w-[90%] mt-10  text-white space-y-3">
            <h1 className="text-5xl font-bold">
              Connect with friends{" "}
              <span className=" text-[#BBC2CC]">seamlessly,</span>{" "}
            </h1>
            <h4 className="text-2xl">stay on track with the latest content!</h4>
            <Image
              src={"/task2.svg"}
              alt="task"
              width={500}
              height={500}
              className="py-20"
            />
          </div>
        </div>
        <div className="bg-white min-h-screen md:w-[60%] md:rounded-md w-full flex flex-col justify-center">
          <h2 className="mt-20 text-center text-3xl md:text-4xl font-bold leading-9 tracking-tight text-gray-900">
            Create your account
          </h2>
          <p className="mt-1 text-center text-1xl font-bold leading-9 tracking-tight text-gray-900">
            Already have an account?
            <Link href="/login" className="text-primary underline ml-1">
              Login
            </Link>
          </p>

          <div className="px-12 py-10 md:px-28">
            <Form {...form}>
              <form onSubmit={form.handleSubmit(onSubmit)}>
                <div className="grid md:grid-cols-2 md:gap-6">
                  <FormField
                    control={form.control}
                    name="firstname"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>First Name</FormLabel>
                        <FormControl>
                          <Input type="text" disabled={isLoading} {...field} />
                        </FormControl>
                        <FormMessage className="text-xs" />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="lastname"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Last Name</FormLabel>
                        <FormControl>
                          <Input type="text" disabled={isLoading} {...field} />
                        </FormControl>
                        <FormMessage className="text-xs" />
                      </FormItem>
                    )}
                  />
                </div>
                <div className="grid py-5">
                  <FormField
                    control={form.control}
                    name="email"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Email Address</FormLabel>
                        <FormControl>
                          <Input type="email" disabled={isLoading} {...field} />
                        </FormControl>
                        <FormMessage className="text-xs" />
                      </FormItem>
                    )}
                  />
                </div>
                <div className="grid md:grid-cols-2 md:gap-6 py-2">
                  <FormField
                    control={form.control}
                    name="password"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Password</FormLabel>
                        <FormControl>
                          <Input
                            type="password"
                            disabled={isLoading}
                            {...field}
                          />
                        </FormControl>
                        <FormMessage className="text-xs" />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="confirmPassword"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Confirm Password</FormLabel>
                        <FormControl>
                          <Input
                            type="password"
                            disabled={isLoading}
                            {...field}
                          />
                        </FormControl>
                        <FormMessage className="text-xs" />
                      </FormItem>
                    )}
                  />
                </div>
                <div className=" py-10">
                  <Button
                    disabled={isLoading}
                    className="w-full py-6"
                    type="submit"
                  >
                    {isLoading ? (
                      <div className="flex gap-2 items-center">
                        Loading <Loader2 className=" animate-spin" />
                      </div>
                    ) : (
                      "Create my account"
                    )}
                  </Button>
                </div>
              </form>
            </Form>
            <AuthDialog
              title={registrationResult?.status}
              description={registrationResult?.message}
              open={isOpen}
              onOpenChange={() => setIsOpen(!isOpen)}
            />
          </div>
        </div>
      </section>
    </div>
  );
};

export default RegisterPage;
