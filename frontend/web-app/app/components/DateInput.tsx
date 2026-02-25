
import { UseControllerProps, useController } from 'react-hook-form'
import DatePicker, { DatePickerProps } from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { HelperText } from 'flowbite-react';

type Props = {
    label: string
    type?: string
    showLabel?: boolean
} & UseControllerProps & DatePickerProps

export default function DateInput(props: Props) {
    const { fieldState, field } = useController({ ...props, defaultValue: '' })

    return (
        <div className='mb-3 block'>
            <DatePicker
                {...props}
                {...field}
                selected={field.value}
                placeholderText={props.label}
                className={`
                        rounded-lg w-full flex flex-col
                        border
                        border-gray-600
                        p-2
                        ${fieldState.error
                        ? 'bg-red-50 border-red-500 text-red-900'
                        : (!fieldState.invalid && fieldState.isDirty)
                            ? 'bg-green-50 border-green-500 text-green-900' : ''}
                    `}
            />
            <HelperText color='failure'>
                {fieldState.error?.message as string}
            </HelperText>
        </div>
    )
}
